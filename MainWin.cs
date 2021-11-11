using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlayingCards;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using GameStatistics;

/*
 * Primary class defines the partial class of the main window for the
 * Pyramid Solitaire game.
 *
 * Author:  M. G. Slack
 * Written: 2013-12-06
 * Version: 1.0.10.0
 *
 * ----------------------------------------------------------------------------
 *
 * Updated: 2013-12-28 - Added a 'play hint' plus an option to turn it off.
 *          2014-03-10 - Added a second high score. One high score is kept
 *                       for the current gaming session only, the second is
 *                       a persisted 'highestScore'.
 *          2014-03-19 - Added the 'GameStatistics' library to track several
 *                       statistics (games won/lost, etc.). Highest score
 *                       tracking is tracked via the statistics class.
 *          2014-06-14 - Updated 'previous card stack' to be a Stack rather
 *                       than a List.
 *          2015-01-31 - Updated the 'win' check to use a 'or' rather than
 *                       an 'and' - win if all cards used up or the pyramid
 *                       all played.  Changed win bonus points to a const
 *                       and made the same for both conditions.
 *          2021-01-12 - Added function to display all cards in pyramid at
 *                       game end (if lost).  May make this optional with
 *                       setting, only shows if cards are not shown in pyramid.
 *          2021-01-20 - Added option to allow card pile to be flipped till no
 *                       more plays can be made.
 *          2021-02-13 - Fixed issue with check end check running twice at
 *                       game end.  Fixed edge condition where end game check
 *                       happened before enabling left over cards in pyramid.
 *                       Prevented winning game clearing pyramid and stock
 *                       cards.
 *          2021-02-25 - Added display of used + up card count.
 *          2021-05-06 - Added set of 'can play' to check if can play, even
 *                       if you don't (end of game check).
 *
 */
namespace PyramidSolitaire
{
    public partial class MainWin : Form
    {
        #region consts
        private const string HTML_HELP_FILE = "PyramidSolitaire_help.html";
        private const int NUM_CARDS = 28;
        private const int POINTS = 13;
        private const int UPCARD_IDX = 30;
        private const int WIN_BONUS = 4;

        // registry name/keys
        private const string REG_NAME = @"HKEY_CURRENT_USER\Software\Slack and Associates\Games\PyramidSolitaire";
        private const string REG_KEY1 = "PosX";
        private const string REG_KEY2 = "PosY";
        private const string REG_KEY3 = "CardBack";
        private const string REG_KEY4 = "CardsUp";
        private const string REG_KEY5 = "ShowHint";
        private const string REG_KEY6 = "FlipPile";
        #endregion

        #region Private Variable/Fields
        private PlayingCardImage images = new PlayingCardImage();
        private CardDeck cards = new CardDeck();
        private PlayingCard upCard = PlayingCard.EMPTY_CARD;
        private CardBacks cardBack = CardBacks.Spheres;
        private bool cardsUp = true, showHint = true, flipPile = false, playing = false, played = true;
        private int firstSelection = -1, highScore = 0, curScore = 0;
        private Pen borderPen = new Pen(Color.Blue, 3);
        private Stack<PlayingCard> stockPile = new Stack<PlayingCard>();
        private Stack<PlayingCard> prevUpCards = new Stack<PlayingCard>();
        private PictureBox[] gameBoardDisplay = new PictureBox[NUM_CARDS];
        private PlayingCard[] gameBoard = new PlayingCard[NUM_CARDS];
        private Statistics stats = new Statistics(REG_NAME);

        // Could've used just the first idx number and a one dimension array
        // (first + 1 = second idx number.  Seems easier to understand by using
        // both idx numbers of the covering cards in the pyramid though.
        private const int ENAB_POS = 21;
        private static readonly int[,] ENABLEMENT = { {1, 2},
                                                  {3, 4}, {4, 5},
                                              {6, 7}, {7, 8}, {8, 9},
                                      {10, 11}, {11, 12}, {12, 13}, {13, 14},
                                 {15, 16}, {16, 17}, {17, 18}, {18, 19}, {19, 20},
                            {21, 22}, {22, 23}, {23, 24}, {24, 25}, {25, 26}, {26, 27} };
        #endregion

        // --------------------------------------------------------------------

        #region Private Methods
        private void LoadRegistryValues()
        {
            int winX = -1, winY = -1, cardB = (int) CardBacks.Spheres;
            string cu_sh;

            try {
                winX = (int) Registry.GetValue(REG_NAME, REG_KEY1, winX);
                winY = (int) Registry.GetValue(REG_NAME, REG_KEY2, winY);
                cardB = (int) Registry.GetValue(REG_NAME, REG_KEY3, cardB);
                cu_sh = (string) Registry.GetValue(REG_NAME, REG_KEY4, "True");
                if (cu_sh != null) cardsUp = Convert.ToBoolean(cu_sh);
                cu_sh = (string) Registry.GetValue(REG_NAME, REG_KEY5, "True");
                if (cu_sh != null) showHint = Convert.ToBoolean(cu_sh);
                cu_sh = (string) Registry.GetValue(REG_NAME, REG_KEY6, "False");
                if (cu_sh != null) flipPile = Convert.ToBoolean(cu_sh);
            }
            catch (Exception ex) { /* ignore, go with defaults */ }

            if ((winX != -1) && (winY != -1)) this.SetDesktopLocation(winX, winY);
            if (Enum.IsDefined(typeof(CardBacks), cardB)) cardBack = (CardBacks) cardB;
            lblHighestScore.Text = Convert.ToString(stats.HighestScore);
        }

        private void SetupContextMenu()
        {
            ContextMenu mnu = new ContextMenu();
            MenuItem mnuStats = new MenuItem("Game Statistics");
            MenuItem sep = new MenuItem("-");
            MenuItem mnuAbout = new MenuItem("About");

            mnuStats.Click += new EventHandler(mnuStats_Click);
            mnuAbout.Click += new EventHandler(mnuAbout_Click);
            mnu.MenuItems.AddRange(new MenuItem[] { mnuStats, sep, mnuAbout });
            this.ContextMenu = mnu;
            PyramidArea.ContextMenu = mnu;
        }

        private void InitBoardDisplay()
        {
            gameBoardDisplay[0] = pbPyra1; gameBoardDisplay[1] = pbPyra2;
            gameBoardDisplay[2] = pbPyra3; gameBoardDisplay[3] = pbPyra4;
            gameBoardDisplay[4] = pbPyra5; gameBoardDisplay[5] = pbPyra6;
            gameBoardDisplay[6] = pbPyra7; gameBoardDisplay[7] = pbPyra8;
            gameBoardDisplay[8] = pbPyra9; gameBoardDisplay[9] = pbPyra10;
            gameBoardDisplay[10] = pbPyra11; gameBoardDisplay[11] = pbPyra12;
            gameBoardDisplay[12] = pbPyra13; gameBoardDisplay[13] = pbPyra14;
            gameBoardDisplay[14] = pbPyra15; gameBoardDisplay[15] = pbPyra16;
            gameBoardDisplay[16] = pbPyra17; gameBoardDisplay[17] = pbPyra18;
            gameBoardDisplay[18] = pbPyra19; gameBoardDisplay[19] = pbPyra20;
            gameBoardDisplay[20] = pbPyra21; gameBoardDisplay[21] = pbPyra22;
            gameBoardDisplay[22] = pbPyra23; gameBoardDisplay[23] = pbPyra24;
            gameBoardDisplay[24] = pbPyra25; gameBoardDisplay[25] = pbPyra26;
            gameBoardDisplay[26] = pbPyra27; gameBoardDisplay[27] = pbPyra28;
        }

        private void ResetGame()
        {
            pbCards.Image = images.GetCardBackImage(cardBack);
            pbUpCard.Image = images.GetCardPlaceholderImage(CardPlaceholders.GreenCircle);

            for (int i = 0; i < NUM_CARDS; i++) {
                gameBoardDisplay[i].Image = null;
                gameBoardDisplay[i].Visible = true;
                gameBoardDisplay[i].Enabled = false;
                gameBoard[i] = PlayingCard.EMPTY_CARD;
            }
        }

        private void ChangeBorder(PictureBox box, Pen pen, bool justClear)
        {
            Graphics objGraphics = null;
            int left = box.Left - 1;
            int top = box.Top - 1;

            if (box != pbUpCard) {
                objGraphics = PyramidArea.CreateGraphics();
                objGraphics.Clear(PyramidArea.BackColor);
            }
            else {
                objGraphics = this.CreateGraphics();
                objGraphics.Clear(this.BackColor);
            }

            if (!justClear) objGraphics.DrawRectangle(pen, left, top, box.Width + 1, box.Height + 1);
            objGraphics.Dispose();
        }

        private void ShowScore()
        {
            if (curScore > highScore) {
                highScore = curScore;
                lblHighScore.Text = Convert.ToString(highScore);
                if (highScore > stats.HighestScore) {
                    lblHighestScore.Text = Convert.ToString(highScore);
                }
            }
            lblCurScore.Text = Convert.ToString(curScore);
        }

        private void ShowCardCounts()
        {
            int usedCnt = prevUpCards.Count;

            lblCardsLeft.Text = Convert.ToString(stockPile.Count);
            if (upCard != PlayingCard.EMPTY_CARD) usedCnt++;
            lblUsedCnt.Text = Convert.ToString(usedCnt);
        }

        private void FlipStockPile()
        {
			played = false;
            while (prevUpCards.Count > 0) stockPile.Push(prevUpCards.Pop());
            ShowCardCounts();
            if (stockPile.Count > 0)
                pbCards.Image = images.GetCardBackImage(cardBack);
        }

        private bool GE_CheckUpCard13()
        {
            if (upCard != PlayingCard.EMPTY_CARD) {
                int cVal = (int) upCard.CardValue;

                if (cVal == POINTS) return false;

                for (int i = 0; i < NUM_CARDS; i++) {
                    if (gameBoardDisplay[i].Enabled) {
                        if ((cVal + (int) gameBoard[i].CardValue) == POINTS) return false;
                    }
                }
            }
            return true;
        }

        private bool GE_CheckBoardCards13()
        {
            for (int i = 0; i < NUM_CARDS - 1; i++) {
                if (gameBoardDisplay[i].Enabled) {
                    int iVal = (int) gameBoard[i].CardValue;

                    if (iVal == POINTS) return false;

                    for (int j = i + 1; j < NUM_CARDS; j++) {
                        if ((i != j) && (gameBoardDisplay[j].Enabled)) {
                            int jVal = (int) gameBoard[j].CardValue;

                            if ((jVal == POINTS) || ((iVal + jVal) == POINTS)) return false;
                        }
                    }
                }
            }
            return true;
        }

        private void ShowBoardCards()
        {
            if (!cardsUp) {
                for (int i = 0; i < NUM_CARDS; i++) {
                    if (gameBoard[i] != PlayingCard.EMPTY_CARD) {
                        gameBoardDisplay[i].Image = images.GetCardImage(gameBoard[i]);
                    }
                }
            }
        }

        private void GE_CheckWon()
        {
            bool won = (gameBoard[0] == PlayingCard.EMPTY_CARD) || // was &&
                       (upCard == PlayingCard.EMPTY_CARD);

            if (gameBoard[0] == PlayingCard.EMPTY_CARD) {
                curScore = curScore + WIN_BONUS;
                ShowScore();
            }
            if (upCard == PlayingCard.EMPTY_CARD) {
                curScore = curScore + WIN_BONUS;
                ShowScore();
            }
            // clear out waiting update events before showing game end dialog
            // mainly to hide the 'can play' hint if visible before game end check...
            Application.DoEvents();
            if (won) {
                stats.GameWon(curScore);
                MessageBox.Show(this, "Congratulations, you've won!", "Game Over",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                ShowBoardCards();
                stats.GameLost(curScore);
                MessageBox.Show(this, "No more plays possible!", "Game Over",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            playing = false;
        }

        private void CheckGameEnd()
        {
            bool gameOver = stockPile.Count == 0;

            // flip stock pile if out of cards in stock and option set
            if (gameOver && flipPile && played) { FlipStockPile(); }

            // reset gameover flag, need to check all conditions in case flipping stock pile
            gameOver = (stockPile.Count == 0) ||
                       (gameBoard[0] == PlayingCard.EMPTY_CARD) ||
                       (upCard == PlayingCard.EMPTY_CARD);

            if (gameOver) {
                // check upcard to cards in board
                gameOver = GE_CheckUpCard13();
                // check cards in board to each other
                if (gameOver) { gameOver = GE_CheckBoardCards13(); }
            }

            if (gameOver) { GE_CheckWon(); }
        }

        private void CanPlayHint()
        {
            bool noPlay = GE_CheckUpCard13();

            if (noPlay) noPlay = GE_CheckBoardCards13();

            // reworked method to set 'played' flag if can play
            // part of reset stock cards update, allows player
            //  to loop through cards and then again if missed
            //  a play that could have been made
            // before, all set of noPlay done inside showHint block
            if (!played) played = !noPlay;

            if (showHint)
            {
                if (noPlay)
                    lblPlayHint.Visible = false;
                else
                    lblPlayHint.Visible = true;
            }
        }

        private void PlayUpCard(PictureBox pb)
        {
            curScore++; played = true;
            ChangeBorder(pb, borderPen, true);
            if (prevUpCards.Count > 0) {
                upCard = prevUpCards.Pop();
                pbUpCard.Image = images.GetCardImage(upCard);
                ShowCardCounts();
            }
            else {
                if (stockPile.Count == 0) {
                    upCard = PlayingCard.EMPTY_CARD;
                    pbUpCard.Image = images.GetCardPlaceholderImage(CardPlaceholders.GreenCircle);
                }
                pbCards_Click(pb, null);
            }
        }

        private void PlayBoardCard(PictureBox pb, int idx)
        {
            curScore++; played = true;
            ChangeBorder(pb, borderPen, true);
            pb.Image = null;
            pb.Enabled = false;
            pb.Visible = false;
            gameBoard[idx] = PlayingCard.EMPTY_CARD;
        }

        private void EnableBoardCards(int idx)
        {
            for (int i = 0; i < ENAB_POS; i++) {
                if (gameBoardDisplay[i].Visible) {
                    int idx1 = ENABLEMENT[i, 0];
                    int idx2 = ENABLEMENT[i, 1];
                    bool enab = !gameBoardDisplay[idx1].Visible &&
                                !gameBoardDisplay[idx2].Visible;

                    if ((enab) && (!gameBoardDisplay[i].Enabled)) {
                        gameBoardDisplay[i].Enabled = enab;
                        if (!cardsUp) gameBoardDisplay[i].Image = images.GetCardImage(gameBoard[i]);
                    }
                }
            }
        }

        private void PlayKing(PictureBox pb, int idx)
        {
            if (pb == pbUpCard)
                PlayUpCard(pb);
            else {
                PlayBoardCard(pb, idx);
                EnableBoardCards(idx);
            }
            firstSelection = -1;
        }

        private void Play13(PictureBox pb, PlayingCard card, int idx)
        {
            PlayingCard firstCard = (firstSelection == UPCARD_IDX ? upCard : gameBoard[firstSelection]);
            int cardVals = (int) card.CardValue + (int) firstCard.CardValue;

            if (cardVals == POINTS) {
                if (firstSelection != UPCARD_IDX)
                    PlayBoardCard(gameBoardDisplay[firstSelection], firstSelection);
                if (pb != pbUpCard)
                    PlayBoardCard(pb, idx);
                EnableBoardCards(idx);
                if ((firstSelection == UPCARD_IDX) || (pb == pbUpCard))
                    PlayUpCard(pbUpCard);
                firstSelection = -1;
            }
            else {
                if (firstSelection == UPCARD_IDX)
                    ChangeBorder(pbUpCard, borderPen, true);
                else
                    ChangeBorder(gameBoardDisplay[firstSelection], borderPen, true);
                firstSelection = idx;
            }
        }

        private void PlayCard(PictureBox pb, PlayingCard card, int idx)
        {
            if (card.CardValue == CardValue.King)
                PlayKing(pb, idx);
            else {
                if (firstSelection == -1)
                    firstSelection = idx;
                else if (firstSelection == idx) {
                    firstSelection = -1;
                    ChangeBorder(pb, borderPen, true);
                }
                else
                    Play13(pb, card, idx);
            }

            ShowScore();
            CanPlayHint();
            if (playing) { CheckGameEnd(); }
        }
        #endregion

        // --------------------------------------------------------------------

        public MainWin()
        {
            InitializeComponent();
        }

        // --------------------------------------------------------------------

        #region Event Handlers
        private void MainWin_Load(object sender, EventArgs e)
        {
            LoadRegistryValues();
            InitBoardDisplay();
            ResetGame();
            SetupContextMenu();
            stats.GameName = this.Text;
        }

        private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) {
                Registry.SetValue(REG_NAME, REG_KEY1, this.Location.X);
                Registry.SetValue(REG_NAME, REG_KEY2, this.Location.Y);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ResetGame();
            cards.Shuffle();

            // deal
            for (int i = 0; i < NUM_CARDS; i++) {
                gameBoard[i] = cards.GetNextCard();
                if ((i > 20) || (cardsUp)) // always display bottom row of cards...
                    gameBoardDisplay[i].Image = images.GetCardImage(gameBoard[i]);
                else
                    gameBoardDisplay[i].Image = images.GetCardBackImage(cardBack);
                if (i > 20) gameBoardDisplay[i].Enabled = true;
            }
            upCard = cards.GetNextCard();
            pbUpCard.Image = images.GetCardImage(upCard);
            stockPile.Clear();
            while (cards.HasMoreCards()) stockPile.Push(cards.GetNextCard());
            playing = true; firstSelection = -1; curScore = 0;
            prevUpCards.Clear();
            ShowScore();
            ShowCardCounts();
            CanPlayHint();
            stats.StartGame(true);
        }

        private void btnOpts_Click(object sender, EventArgs e)
        {
            OptionsWin opts = new OptionsWin();

            opts.Images = images;
            opts.CardBack = cardBack;
            opts.ShowCards = cardsUp;
            opts.ShowHint = showHint;
            opts.FlipPile = flipPile;

            if (opts.ShowDialog(this) == DialogResult.OK) {
                cardBack = opts.CardBack;
                cardsUp = opts.ShowCards;
                showHint = opts.ShowHint;
                flipPile = opts.FlipPile;

                Registry.SetValue(REG_NAME, REG_KEY3, (int) cardBack);
                Registry.SetValue(REG_NAME, REG_KEY4, cardsUp);
                Registry.SetValue(REG_NAME, REG_KEY5, showHint);
                Registry.SetValue(REG_NAME, REG_KEY6, flipPile);

                if (!showHint)
                    lblPlayHint.Visible = false;
                else
                    CanPlayHint();
            }

            opts.Dispose();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var asm = Assembly.GetEntryAssembly();
            var asmLocation = Path.GetDirectoryName(asm.Location);
            var htmlPath = Path.Combine(asmLocation, HTML_HELP_FILE);

            try {
                Process.Start(htmlPath);
            }
            catch (Exception ex) {
                MessageBox.Show("Cannot load help: " + ex.Message, "Help Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuStats_Click(object sender, EventArgs e)
        {
            stats.ShowStatistics(this);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();

            about.ShowDialog(this);
            about.Dispose();
        }

        private void pbCards_Click(object sender, EventArgs e)
        {
            if ((playing) && (stockPile.Count > 0)) {
                if (sender == pbCards) {
                    prevUpCards.Push(upCard);
                    firstSelection = -1;
                }
                upCard = stockPile.Pop();
                pbUpCard.Image = images.GetCardImage(upCard);
                ShowCardCounts();
                if (stockPile.Count == 0)
                    pbCards.Image = images.GetCardPlaceholderImage(CardPlaceholders.RedX);
                ChangeBorder(pbUpCard, borderPen, true);
                ChangeBorder(pbPyra1, borderPen, true);
                CanPlayHint();
                CheckGameEnd();
            }
        }

        private void pbPyraX_Click(object sender, EventArgs e)
        {
            if (playing) {
                PictureBox pb = (PictureBox) sender;
                int idx = Convert.ToInt32(pb.Tag);
                PlayingCard card = PlayingCard.EMPTY_CARD;

                if (idx == UPCARD_IDX)
                    card = upCard;
                else
                    card = gameBoard[idx];
                ChangeBorder(pb, borderPen, false);
                PlayCard(pb, card, idx);
            }
        }
        #endregion
    }
}
