using System;
using System.Windows.Forms;
using PlayingCards;

/*
 * Primary class defines the partial class of the options dialog for the
 * Pyramid Solitaire game.
 *
 * Author:  M. G. Slack
 * Written: 2013-12-08
 *
 * ----------------------------------------------------------------------------
 * 
 * Updated: 2013-12-28 - Added 'can play' checkbox to dialog to control if the
 *                       'can play' hint is shown or not.
 *          2021-01-20 - Added 'flip pile' checkbox to dialog to control if the
 *                       card pile is flipped over when used up or not.
 *
 */
namespace PyramidSolitaire
{
    public partial class OptionsWin : Form
    {
        #region Properties
        private CardBacks _cardBack = CardBacks.Spheres;
        public CardBacks CardBack { get { return _cardBack; } set { _cardBack = value; } }

        private bool _showCards = true;
        public bool ShowCards { get { return _showCards; } set { _showCards = value; } }

        private bool _showHint = true;
        public bool ShowHint { get { return _showHint; } set { _showHint = value; } }

        private bool _flipPile = false;
        public bool FlipPile { get { return _flipPile; } set { _flipPile = value; } }

        private PlayingCardImage _images = null;
        public PlayingCardImage Images { set { _images = value; } }
        #endregion

        // --------------------------------------------------------------------

        public OptionsWin()
        {
            InitializeComponent();
        }

        // --------------------------------------------------------------------

        #region Event Handlers
        private void OptionsWin_Load(object sender, EventArgs e)
        {
            int idx = 0;
            
            cbShowCards.Checked = _showCards;
            cbShowHint.Checked = _showHint;
            cbFlipPile.Checked = _flipPile;

            foreach (string name in Enum.GetNames(typeof(CardBacks))) {
                cbImage.Items.Add(name);
            }

            foreach (int val in Enum.GetValues(typeof(CardBacks))) {
                if (val == (int) _cardBack) {
                    idx = (int) _cardBack - (int) CardBacks.Spheres;
                }
            }
            cbImage.SelectedIndex = idx;

            if (_images != null) {
                pbBack.Image = _images.GetCardBackImage(_cardBack);
            }
        }

        private void cbShowCards_CheckedChanged(object sender, EventArgs e)
        {
            _showCards = cbShowCards.Checked;
        }

        private void cbShowHint_CheckedChanged(object sender, EventArgs e)
        {
            _showHint = cbShowHint.Checked;
        }

        private void cbFlipPile_CheckedChanged(object sender, EventArgs e)
        {
            _flipPile = cbFlipPile.Checked;
        }

        private void cbImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _cardBack = (CardBacks) (cbImage.SelectedIndex + (int) CardBacks.Spheres);
            if (_images != null) {
                pbBack.Image = _images.GetCardBackImage(_cardBack);
            }
        }
        #endregion
    }
}
