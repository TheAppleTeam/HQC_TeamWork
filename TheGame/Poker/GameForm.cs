﻿namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class GameForm : Form
    {
        #region Constants

        private const int InitialBigBlind = 500;
        private const int InitialSmallBlind = 250;
        private const int BotCount = 5;
        private const int InitialChips = 10000;
        private const int DealtCards = 17;



        #endregion
        #region Variables
        private int pokerCall = InitialBigBlind;
        private int foldedPlayers = BotCount;
        private int playerChips = InitialChips;
        private int bot1Chips = InitialChips;
        private int bot2Chips = InitialChips;
        private int bot3Chips = InitialChips;
        private int bot4Chips = InitialChips;
        private int bot5Chips = InitialChips;

        private readonly Panel playerCardsPanel = new Panel();
        private readonly Panel bot1CardsPanel = new Panel();
        private readonly Panel bot2CardsPanel = new Panel();
        private readonly Panel bot3CardsPanel = new Panel();
        private readonly Panel bot4CardsPanel = new Panel();
        private readonly Panel bot5CardsPanel = new Panel();

        private double type;
        private double rounds;
        private double bot1Power;
        private double bot2Power;
        private double bot3Power;
        private double bot4Power;
        private double bot5Power;
        private double playerPower;
        private double playerType = -1;
        private double bot1Type = -1;
        private double bot2Type = -1;
        private double bot3Type = -1;
        private double bot4Type = -1;
        private double bot5Type = -1;
        private double raise;
        private bool bot1Turn = false;
        private bool bot2Turn = false;
        private bool bot3Turn = false;
        private bool bot4Turn = false;
        private bool bot5Turn = false;

        // is set TRUE when bot's chips (bot{num}Chips) goes <=0 
        private bool bot1GameEnded = false;
        private bool bot2GameEnded = false;
        private bool bot3GameEnded = false;
        private bool bot4GameEnded = false;
        private bool bot5GameEnded = false;

        private bool playerFolded;
        private bool bot1Folded;
        private bool bot2Folded;
        private bool bot3Folded;
        private bool bot4Folded;
        private bool bot5Folded;

        private bool intsadded;
        private bool changed;

        private int playerCall = 0;
        private int bot1Call = 0;
        private int bot2Call = 0;
        private int bot3Call = 0;
        private int bot4Call = 0;
        private int bot5Call = 0;
        private int playerRaise = 0;
        private int bot1Raise = 0;
        private int bot2Raise = 0;
        private int bot3Raise = 0;
        private int bot4Raise = 0;
        private int bot5Raise = 0;

        private int height;
        private int width;

        private int winnersCount = 0;
        //used in CheckRaise method :   if (rounds == Flop); in Finish method is seted again on 1;
        private int Flop = 1;
        //used in CheckRaise method :  if (rounds == Turn); in Finish method is seted again on 2;
        private int Turn = 2;
        private int River = 3; 
        // used in CheckRaise method :  (rounds == River); in Finish method is seted again on 3;
        private int End = 4; 
        // used in CheckRaise method :   if (rounds == End && maxPlayersLeftCount == 6);
        //used in AllIn method->  #region FiveOrLessLeft: if (abc < 6 && abc > 1 && rounds >= End) 
        //in Finish method is seted again on 4;
        private int maxPlayersLeftCount = 6;
        //used in Turns method -> region Rotating : in every positive check is game ending  maxPlayersLeftCount--;
                        // used in CheckRaise method :   if (rounds == End && maxPlayersLeftCount == 6); and  if (turnCount >= maxPlayersLeftCount - 1 || !changed && turnCount == maxPlayersLeftCount);
                         //used in AllIn method : if (ints.ToArray().Length == maxPlayersLeftCount)
                         //in Finish method is seted again on 6;
        private int lastBotPlayed = 123; 
        int raisedTurn = 1;
        List<bool?> bools = new List<bool?>();
        List<Type> Win = new List<Type>();
        List<string> CheckWinners = new List<string>();
        List<int> ints = new List<int>();
        bool playerGameEnded = false, Pturn = true, restart = false, raising = false;
        Poker.Type sorted;

    

        string[] ImgLocation = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
        /*string[] ImgLocation ={
                   "Assets\\Cards\\33.png","Assets\\Cards\\22.png",
                    "Assets\\Cards\\29.png","Assets\\Cards\\21.png",
                    "Assets\\Cards\\36.png","Assets\\Cards\\17.png",
                    "Assets\\Cards\\40.png","Assets\\Cards\\16.png",
                    "Assets\\Cards\\5.png","Assets\\Cards\\47.png",
                    "Assets\\Cards\\37.png","Assets\\Cards\\13.png",
                    
                    "Assets\\Cards\\12.png",
                    "Assets\\Cards\\8.png","Assets\\Cards\\18.png",
                    "Assets\\Cards\\15.png","Assets\\Cards\\27.png"};*/
        int[] dealtCards = new int[17];
        Image[] Deck = new Image[52];
        PictureBox[] Holder = new PictureBox[52];
        Timer timer = new Timer();
        Timer Updates = new Timer();
        int t = 60, i, bb = InitialBigBlind, sb = InitialSmallBlind, up = 10000000, turnCount = 0;
        #endregion
        public GameForm()
        {
            //bools.Add(PFturn); bools.Add(B1Fturn); bools.Add(bot2GameEnded); bools.Add(bot3GameEnded); bools.Add(bot4GameEnded); bools.Add(bot5GameEnded);
            this.pokerCall = InitialBigBlind;
            MaximizeBox = false;
            MinimizeBox = false;
            Updates.Start();
            InitializeComponent();
            width = this.Width;
            height = this.Height;
            Shuffle();
            textBoxPot.Enabled = false;
            textBoxPlayerChips.Enabled = false;
            textBoxBot1Chips.Enabled = false;
            textBoxBot2Chips.Enabled = false;
            textBoxBot3Chips.Enabled = false;
            textBoxBot4Chips.Enabled = false;
            textBoxBot5Chips.Enabled = false;
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
            textBoxBot1Chips.Text = "Chips : " + bot1Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + bot2Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + bot3Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + bot4Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + bot5Chips.ToString();
            timer.Interval = (1 * 1 * 1000);
            timer.Tick += timer_Tick;
            Updates.Interval = (1 * 1 * 100);
            Updates.Tick += Update_Tick;
            textBoxBigBlind.Visible = true;
            textBoxSmallBlind.Visible = true;
            buttonBigBlind.Visible = true;
            buttonSmallBlind.Visible = true;
            textBoxBigBlind.Visible = true;
            textBoxSmallBlind.Visible = true;
            buttonBigBlind.Visible = true;
            buttonSmallBlind.Visible = true;
            textBoxBigBlind.Visible = false;
            textBoxSmallBlind.Visible = false;
            buttonBigBlind.Visible = false;
            buttonSmallBlind.Visible = false;
            textBoxRaise.Text = (bb * 2).ToString();
        }
        async Task Shuffle()
        {
            bools.Add(this.playerGameEnded);
            bools.Add(this.bot1GameEnded);
            bools.Add(this.bot2GameEnded);
            bools.Add(this.bot3GameEnded);
            bools.Add(this.bot4GameEnded);
            bools.Add(this.bot5GameEnded);
            buttonCall.Enabled = false;
            buttonRaise.Enabled = false;
            buttonFold.Enabled = false;
            buttonCheck.Enabled = false;
            MaximizeBox = false;
            MinimizeBox = false;
            bool check = false;
            Bitmap backImage = new Bitmap("Assets\\Back\\Back.png");
            int horizontal = 580, vertical = 480;
            Random r = new Random();
            for (i = ImgLocation.Length; i > 0; i--)
            {
                int j = r.Next(i);
                var k = ImgLocation[j];
                ImgLocation[j] = ImgLocation[i - 1];
                ImgLocation[i - 1] = k;
            }
            for (i = 0; i < DealtCards; i++)
            {
                //"Assets\\Cards\\1.png"
                //1
                Deck[i] = Image.FromFile(ImgLocation[i]);
                var charsToRemove = new string[] { "Assets\\Cards\\", ".png" };
                foreach (var c in charsToRemove)
                {
                    ImgLocation[i] = ImgLocation[i].Replace(c, string.Empty);
                }
                this.dealtCards[i] = int.Parse(ImgLocation[i]) - 1;
                Holder[i] = new PictureBox();
                Holder[i].SizeMode = PictureBoxSizeMode.StretchImage;
                Holder[i].Height = 130;
                Holder[i].Width = 80;
                this.Controls.Add(Holder[i]);
                Holder[i].Name = "pb" + i.ToString();
                await Task.Delay(200);
                #region Throwing Cards
                if (i < 2)
                {
                    if (Holder[0].Tag != null)
                    {
                        Holder[1].Tag = this.dealtCards[1];
                    }
                    Holder[0].Tag = this.dealtCards[0];
                    Holder[i].Image = Deck[i];
                    Holder[i].Anchor = (AnchorStyles.Bottom);
                    //Holder[i].Dock = DockStyle.Top;
                    Holder[i].Location = new Point(horizontal, vertical);
                    horizontal += Holder[i].Width;
                    this.Controls.Add(this.playerCardsPanel);
                    this.playerCardsPanel.Location = new Point(Holder[0].Left - 10, Holder[0].Top - 10);
                    this.playerCardsPanel.BackColor = Color.DarkBlue;
                    this.playerCardsPanel.Height = 150;
                    this.playerCardsPanel.Width = 180;
                    this.playerCardsPanel.Visible = false;
                }
                if (this.bot1Chips > 0)
                {
                    foldedPlayers--;
                    if (i >= 2 && i < 4)
                    {
                        if (Holder[2].Tag != null)
                        {
                            Holder[3].Tag = this.dealtCards[3];
                        }
                        Holder[2].Tag = this.dealtCards[2];
                        if (!check)
                        {
                            horizontal = 15;
                            vertical = 420;
                        }
                        check = true;
                        Holder[i].Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += Holder[i].Width;
                        Holder[i].Visible = true;
                        this.Controls.Add(this.bot1CardsPanel);
                        this.bot1CardsPanel.Location = new Point(Holder[2].Left - 10, Holder[2].Top - 10);
                        this.bot1CardsPanel.BackColor = Color.DarkBlue;
                        this.bot1CardsPanel.Height = 150;
                        this.bot1CardsPanel.Width = 180;
                        this.bot1CardsPanel.Visible = false;
                        if (i == 3)
                        {
                            check = false;
                        }
                    }
                }
                if (this.bot2Chips > 0)
                {
                    foldedPlayers--;
                    if (i >= 4 && i < 6)
                    {
                        if (Holder[4].Tag != null)
                        {
                            Holder[5].Tag = this.dealtCards[5];
                        }
                        Holder[4].Tag = this.dealtCards[4];
                        if (!check)
                        {
                            horizontal = 75;
                            vertical = 65;
                        }
                        check = true;
                        Holder[i].Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += Holder[i].Width;
                        Holder[i].Visible = true;
                        this.Controls.Add(this.bot2CardsPanel);
                        this.bot2CardsPanel.Location = new Point(Holder[4].Left - 10, Holder[4].Top - 10);
                        this.bot2CardsPanel.BackColor = Color.DarkBlue;
                        this.bot2CardsPanel.Height = 150;
                        this.bot2CardsPanel.Width = 180;
                        this.bot2CardsPanel.Visible = false;
                        if (i == 5)
                        {
                            check = false;
                        }
                    }
                }
                if (bot3Chips > 0)
                {
                    foldedPlayers--;
                    if (i >= 6 && i < 8)
                    {
                        if (Holder[6].Tag != null)
                        {
                            Holder[7].Tag = this.dealtCards[7];
                        }
                        Holder[6].Tag = this.dealtCards[6];
                        if (!check)
                        {
                            horizontal = 590;
                            vertical = 25;
                        }
                        check = true;
                        Holder[i].Anchor = (AnchorStyles.Top);
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += Holder[i].Width;
                        Holder[i].Visible = true;
                        this.Controls.Add(this.bot3CardsPanel);
                        this.bot3CardsPanel.Location = new Point(Holder[6].Left - 10, Holder[6].Top - 10);
                        this.bot3CardsPanel.BackColor = Color.DarkBlue;
                        this.bot3CardsPanel.Height = 150;
                        this.bot3CardsPanel.Width = 180;
                        this.bot3CardsPanel.Visible = false;
                        if (i == 7)
                        {
                            check = false;
                        }
                    }
                }
                if (bot4Chips > 0)
                {
                    foldedPlayers--;
                    if (i >= 8 && i < 10)
                    {
                        if (Holder[8].Tag != null)
                        {
                            Holder[9].Tag = this.dealtCards[9];
                        }
                        Holder[8].Tag = this.dealtCards[8];
                        if (!check)
                        {
                            horizontal = 1115;
                            vertical = 65;
                        }
                        check = true;
                        Holder[i].Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += Holder[i].Width;
                        Holder[i].Visible = true;
                        this.Controls.Add(this.bot4CardsPanel);
                        this.bot4CardsPanel.Location = new Point(Holder[8].Left - 10, Holder[8].Top - 10);
                        this.bot4CardsPanel.BackColor = Color.DarkBlue;
                        this.bot4CardsPanel.Height = 150;
                        this.bot4CardsPanel.Width = 180;
                        this.bot4CardsPanel.Visible = false;
                        if (i == 9)
                        {
                            check = false;
                        }
                    }
                }
                if (bot5Chips > 0)
                {
                    foldedPlayers--;
                    if (i >= 10 && i < 12)
                    {
                        if (Holder[10].Tag != null)
                        {
                            Holder[11].Tag = this.dealtCards[11];
                        }
                        Holder[10].Tag = this.dealtCards[10];
                        if (!check)
                        {
                            horizontal = 1160;
                            vertical = 420;
                        }
                        check = true;
                        Holder[i].Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += Holder[i].Width;
                        Holder[i].Visible = true;
                        this.Controls.Add(this.bot5CardsPanel);
                        this.bot5CardsPanel.Location = new Point(Holder[10].Left - 10, Holder[10].Top - 10);
                        this.bot5CardsPanel.BackColor = Color.DarkBlue;
                        this.bot5CardsPanel.Height = 150;
                        this.bot5CardsPanel.Width = 180;
                        this.bot5CardsPanel.Visible = false;
                        if (i == 11)
                        {
                            check = false;
                        }
                    }
                }
                if (i >= 12)
                {
                    Holder[12].Tag = this.dealtCards[12];
                    if (i > 12) Holder[13].Tag = this.dealtCards[13];
                    if (i > 13) Holder[14].Tag = this.dealtCards[14];
                    if (i > 14) Holder[15].Tag = this.dealtCards[15];
                    if (i > 15)
                    {
                        Holder[16].Tag = this.dealtCards[16];

                    }
                    if (!check)
                    {
                        horizontal = 410;
                        vertical = 265;
                    }
                    check = true;
                    if (Holder[i] != null)
                    {
                        Holder[i].Anchor = AnchorStyles.None;
                        Holder[i].Image = backImage;
                        //Holder[i].Image = Deck[i];
                        Holder[i].Location = new Point(horizontal, vertical);
                        horizontal += 110;
                    }
                }
                #endregion
                if (bot1Chips <= 0)
                {
                    this.bot1GameEnded = true;
                    Holder[2].Visible = false;
                    Holder[3].Visible = false;
                }
                else
                {
                    this.bot1GameEnded = false;
                    if (i == 3)
                    {
                        if (Holder[3] != null)
                        {
                            Holder[2].Visible = true;
                            Holder[3].Visible = true;
                        }
                    }
                }
                if (bot2Chips <= 0)
                {
                    this.bot2GameEnded = true;
                    Holder[4].Visible = false;
                    Holder[5].Visible = false;
                }
                else
                {
                    this.bot2GameEnded = false;
                    if (i == 5)
                    {
                        if (Holder[5] != null)
                        {
                            Holder[4].Visible = true;
                            Holder[5].Visible = true;
                        }
                    }
                }
                if (bot3Chips <= 0)
                {
                    this.bot3GameEnded = true;
                    Holder[6].Visible = false;
                    Holder[7].Visible = false;
                }
                else
                {
                    this.bot3GameEnded = false;
                    if (i == 7)
                    {
                        if (Holder[7] != null)
                        {
                            Holder[6].Visible = true;
                            Holder[7].Visible = true;
                        }
                    }
                }
                if (bot4Chips <= 0)
                {
                    this.bot4GameEnded = true;
                    Holder[8].Visible = false;
                    Holder[9].Visible = false;
                }
                else
                {
                    this.bot4GameEnded = false;
                    if (i == 9)
                    {
                        if (Holder[9] != null)
                        {
                            Holder[8].Visible = true;
                            Holder[9].Visible = true;
                        }
                    }
                }
                if (bot5Chips <= 0)
                {
                   this.bot5GameEnded = true;
                    Holder[10].Visible = false;
                    Holder[11].Visible = false;
                }
                else
                {
                    this.bot5GameEnded = false;
                    if (i == 11)
                    {
                        if (Holder[11] != null)
                        {
                            Holder[10].Visible = true;
                            Holder[11].Visible = true;
                        }
                    }
                }
                if (i == 16)
                {
                    if (!restart)
                    {
                        MaximizeBox = true;
                        MinimizeBox = true;
                    }
                    timer.Start();
                }
            }
            if (foldedPlayers == 5)
            {
                DialogResult dialogResult = MessageBox.Show("Would You Like To Play Again ?", "You Won , Congratulations ! ", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Application.Restart();
                }
                else if (dialogResult == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            else
            {
                foldedPlayers = 5;
            }
            if (i == 17)
            {
                buttonRaise.Enabled = true;
                buttonCall.Enabled = true;
                buttonFold.Enabled = true;
            }
        }
        async Task Turns()
        {
            #region Rotating
            if (!this.playerGameEnded)
            {
                if (Pturn)
                {
                    FixCall(labelPlayerStatus, ref this.playerCall, ref playerRaise, 1);
                    //MessageBox.Show("Player's Turn");
                    progressBarTimer.Visible = true;
                    progressBarTimer.Value = 1000;
                    t = 60;
                    up = 10000000;
                    timer.Start();
                    buttonRaise.Enabled = true;
                    buttonCall.Enabled = true;
                    buttonRaise.Enabled = true;
                    buttonRaise.Enabled = true;
                    buttonFold.Enabled = true;
                    turnCount++;
                    FixCall(labelPlayerStatus, ref this.playerCall, ref playerRaise, 2);
                }
            }
            
            if (this.playerGameEnded || !Pturn)
            {
                await AllIn();
                if (this.playerGameEnded && !this.playerFolded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                    {
                        bools.RemoveAt(0);
                        bools.Insert(0, null);
                        maxPlayersLeftCount--;
                        this.playerFolded = true;
                    }
                }
                await CheckRaise(0, 0);
                progressBarTimer.Visible = false;
                buttonRaise.Enabled = false;
                buttonCall.Enabled = false;
                buttonRaise.Enabled = false;
                buttonRaise.Enabled = false;
                buttonFold.Enabled = false;
                timer.Stop();
                this.bot1Turn = true;
                if (!this.bot1GameEnded)
                {
                    if (this.bot1Turn)
                    {
                        FixCall(labelBot1Status, ref this.bot1Call, ref bot1Raise, 1);
                        FixCall(labelBot1Status, ref this.bot1Call, ref bot1Raise, 2);
                        Rules(2, 3, "Bot 1", ref this.bot1Type, ref this.bot1Power, this.bot1GameEnded);
                        MessageBox.Show("Bot 1's Turn");
                        AI(2, 3, ref bot1Chips, ref this.bot1Turn, ref this.bot1GameEnded, labelBot1Status, 0, this.bot1Power, this.bot1Type);
                        turnCount++;
                        lastBotPlayed = 1;
                        this.bot1Turn = false;
                        this.bot2Turn = true;
                    }
                }
                if (this.bot1GameEnded && !this.bot1Folded)
                {
                    bools.RemoveAt(1);
                    bools.Insert(1, null);
                    maxPlayersLeftCount--;
                    this.bot1Folded = true;
                }
                if (this.bot1GameEnded || !this.bot1Turn)
                {
                    await CheckRaise(1, 1);
                    this.bot2Turn = true;
                }
                if (!this.bot2GameEnded)
                {
                    if (this.bot2Turn)
                    {
                        FixCall(labelBot2Status, ref this.bot2Call, ref bot2Raise, 1);
                        FixCall(labelBot2Status, ref this.bot2Call, ref bot2Raise, 2);
                        Rules(4, 5, "Bot 2", ref this.bot2Type, ref this.bot2Power, this.bot2GameEnded);
                        MessageBox.Show("Bot 2's Turn");
                        AI(4, 5, ref bot2Chips, ref this.bot2Turn, ref this.bot2GameEnded, labelBot2Status, 1, this.bot2Power, this.bot2Type);
                        turnCount++;
                        lastBotPlayed = 2;
                        this.bot2Turn = false;
                        this.bot3Turn = true;
                    }
                }
                if (this.bot2GameEnded && !this.bot2Folded)
                {
                    bools.RemoveAt(2);
                    bools.Insert(2, null);
                    maxPlayersLeftCount--;
                    this.bot2Folded = true;
                }
                if (this.bot2GameEnded || !this.bot2Turn)
                {
                    await CheckRaise(2, 2);
                    this.bot3Turn = true;
                }
                if (!this.bot3GameEnded)
                {
                    if (this.bot3Turn)
                    {
                        FixCall(labelBot3Status, ref bot3Call, ref bot3Raise, 1);
                        FixCall(labelBot3Status, ref bot3Call, ref bot3Raise, 2);
                        Rules(6, 7, "Bot 3", ref this.bot3Type, ref this.bot3Power, this.bot3GameEnded);
                        MessageBox.Show("Bot 3's Turn");
                        AI(6, 7, ref bot3Chips, ref this.bot3Turn, ref this.bot3GameEnded, labelBot3Status, 2, this.bot3Power, this.bot3Type);
                        turnCount++;
                        lastBotPlayed = 3;
                        this.bot3Turn = false;
                        this.bot4Turn = true;
                    }
                }
                if (this.bot3GameEnded && !this.bot3Folded)
                {
                    bools.RemoveAt(3);
                    bools.Insert(3, null);
                    maxPlayersLeftCount--;
                    this.bot3Folded = true;
                }
                if (this.bot3GameEnded || !this.bot3Turn)
                {
                    await CheckRaise(3, 3);
                    this.bot4Turn = true;
                }
                if (!this.bot4GameEnded)
                {
                    if (this.bot4Turn)
                    {
                        FixCall(labelBot4Status, ref bot4Call, ref bot4Raise, 1);
                        FixCall(labelBot4Status, ref bot4Call, ref bot4Raise, 2);
                        Rules(8, 9, "Bot 4", ref this.bot4Type, ref this.bot4Power, this.bot4GameEnded);
                        MessageBox.Show("Bot 4's Turn");
                        AI(8, 9, ref bot4Chips, ref this.bot4Turn, ref this.bot4GameEnded, labelBot4Status, 3, this.bot4Power, this.bot4Type);
                        turnCount++;
                        lastBotPlayed = 4;
                        this.bot4Turn = false;
                        this.bot5Turn = true;
                    }
                }
                if (this.bot4GameEnded && !this.bot4Folded)
                {
                    bools.RemoveAt(4);
                    bools.Insert(4, null);
                    maxPlayersLeftCount--;
                    this.bot4Folded = true;
                }
                if (bot4GameEnded || !this.bot4Turn)
                {
                    await CheckRaise(4, 4);
                    this.bot5Turn = true;
                }
                if (!bot5GameEnded)
                {
                    if (this.bot5Turn)
                    {
                        FixCall(labelBot5Status, ref bot5Call, ref bot5Raise, 1);
                        FixCall(labelBot5Status, ref bot5Call, ref bot5Raise, 2);
                        Rules(10, 11, "Bot 5", ref this.bot5Type, ref this.bot5Power, bot5GameEnded);
                        MessageBox.Show("Bot 5's Turn");
                        AI(10, 11, ref bot5Chips, ref this.bot5Turn, ref  bot5GameEnded, labelBot5Status, 4, this.bot5Power, this.bot5Type);
                        turnCount++;
                        lastBotPlayed = 5;
                        this.bot5Turn = false;
                    }
                }
                if (bot5GameEnded && !this.bot5Folded)
                {
                    bools.RemoveAt(5);
                    bools.Insert(5, null);
                    maxPlayersLeftCount--;
                    this.bot5Folded = true;
                }
                if (bot5GameEnded || !this.bot5Turn)
                {
                    await CheckRaise(5, 5);
                    Pturn = true;
                }
                if (this.playerGameEnded && !this.playerFolded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                    {
                        bools.RemoveAt(0);
                        bools.Insert(0, null);
                        maxPlayersLeftCount--;
                        this.playerFolded = true;
                    }
                }
            #endregion
                await AllIn();
                if (!restart)
                {
                    await Turns();
                }
                restart = false;
            }
        }

        void Rules(int firstCard, int secondCard, string currentText, ref double current, ref double Power, bool foldedTurn)
        {
            if (firstCard == 0 && secondCard == 1)
            {
            }

            if (!foldedTurn || firstCard == 0 && secondCard == 1 && labelPlayerStatus.Text.Contains("Fold") == false)
            {
                #region Variables
                bool done = false;
                bool vf = false;
                int[] cardsOnTable = new int[5];
                int[] cardsOnTableWithPlayerCards = new int[7];
                cardsOnTableWithPlayerCards[0] = this.dealtCards[firstCard];
                cardsOnTableWithPlayerCards[1] = this.dealtCards[secondCard];

                cardsOnTable[0] = cardsOnTableWithPlayerCards[2] = this.dealtCards[12];
                cardsOnTable[1] = cardsOnTableWithPlayerCards[3] = this.dealtCards[13];
                cardsOnTable[2] = cardsOnTableWithPlayerCards[4] = this.dealtCards[14];
                cardsOnTable[3] = cardsOnTableWithPlayerCards[5] = this.dealtCards[15];
                cardsOnTable[4] = cardsOnTableWithPlayerCards[6] = this.dealtCards[16];

                var clubs = cardsOnTableWithPlayerCards.Where(o => o % 4 == 0).ToArray();
                var diamonds = cardsOnTableWithPlayerCards.Where(o => o % 4 == 1).ToArray();
                var hearts = cardsOnTableWithPlayerCards.Where(o => o % 4 == 2).ToArray();
                var spades = cardsOnTableWithPlayerCards.Where(o => o % 4 == 3).ToArray();

                var cardsOfClubs = clubs.Select(o => o / 4).Distinct().ToArray();
                var cardsOfDiamonds = diamonds.Select(o => o / 4).Distinct().ToArray();
                var cardsOfHearts = hearts.Select(o => o / 4).Distinct().ToArray();
                var cardsOfSpades = spades.Select(o => o / 4).Distinct().ToArray();

                Array.Sort(cardsOnTableWithPlayerCards);
                Array.Sort(cardsOfClubs);
                Array.Sort(cardsOfDiamonds);
                Array.Sort(cardsOfHearts);
                Array.Sort(cardsOfSpades);
                #endregion

                for (i = 0; i < DealtCards - 1; i++)
                {
                    //if (this.dealtCards[i] == int.Parse(Holder[firstCard].Tag.ToString()) && this.dealtCards[i + 1] == int.Parse(Holder[secondCard].Tag.ToString()))
                    //{
                        //Pair from Hand current = 1

                        rPairFromHand(ref current, ref Power, i);

                        #region Pair or Two Pair from Table current = 2 || 0
                        rPairTwoPair(ref current, ref Power);
                        #endregion

                        #region Two Pair current = 2
                        rTwoPair(ref current, ref Power);
                        #endregion

                        #region Three of a kind current = 3
                        rThreeOfAKind(ref current, ref Power, cardsOnTableWithPlayerCards);
                        #endregion

                        #region Straight current = 4
                        rStraight(ref current, ref Power, cardsOnTableWithPlayerCards);
                        #endregion

                        #region Flush current = 5 || 5.5
                        rFlush(ref current, ref Power, ref vf, cardsOnTable);
                        #endregion

                        #region Full House current = 6
                        rFullHouse(ref current, ref Power, ref done, cardsOnTableWithPlayerCards);
                        #endregion

                        #region Four of a Kind current = 7
                        rFourOfAKind(ref current, ref Power, cardsOnTableWithPlayerCards);
                        #endregion

                        #region Straight Flush current = 8 || 9
                        rStraightFlush(ref current, ref Power, cardsOfClubs, cardsOfDiamonds, cardsOfHearts, cardsOfSpades);
                        #endregion

                        #region High Card current = -1
                        rHighCard(ref current, ref Power);
                        #endregion
                    //}
                }
            }
        }
        private void rStraightFlush(ref double current, ref double Power, int[] st1, int[] st2, int[] st3, int[] st4)
        {
            if (current >= -1)
            {
                if (st1.Length >= 5)
                {
                    if (st1[0] + 4 == st1[4])
                    {
                        current = 8;
                        Power = (st1.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 8 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (st1[0] == 0 && st1[1] == 9 && st1[2] == 10 && st1[3] == 11 && st1[0] + 12 == st1[4])
                    {
                        current = 9;
                        Power = (st1.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 9 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (st2.Length >= 5)
                {
                    if (st2[0] + 4 == st2[4])
                    {
                        current = 8;
                        Power = (st2.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 8 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (st2[0] == 0 && st2[1] == 9 && st2[2] == 10 && st2[3] == 11 && st2[0] + 12 == st2[4])
                    {
                        current = 9;
                        Power = (st2.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 9 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (st3.Length >= 5)
                {
                    if (st3[0] + 4 == st3[4])
                    {
                        current = 8;
                        Power = (st3.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 8 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (st3[0] == 0 && st3[1] == 9 && st3[2] == 10 && st3[3] == 11 && st3[0] + 12 == st3[4])
                    {
                        current = 9;
                        Power = (st3.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 9 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (st4.Length >= 5)
                {
                    if (st4[0] + 4 == st4[4])
                    {
                        current = 8;
                        Power = (st4.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 8 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (st4[0] == 0 && st4[1] == 9 && st4[2] == 10 && st4[3] == 11 && st4[0] + 12 == st4[4])
                    {
                        current = 9;
                        Power = (st4.Max()) / 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 9 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFourOfAKind(ref double current, ref double Power, int[] Straight)
        {
            if (current >= -1)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (Straight[j] / 4 == Straight[j + 1] / 4 && Straight[j] / 4 == Straight[j + 2] / 4 &&
                        Straight[j] / 4 == Straight[j + 3] / 4)
                    {
                        current = 7;
                        Power = (Straight[j] / 4) * 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 7 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (Straight[j] / 4 == 0 && Straight[j + 1] / 4 == 0 && Straight[j + 2] / 4 == 0 && Straight[j + 3] / 4 == 0)
                    {
                        current = 7;
                        Power = 13 * 4 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 7 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFullHouse(ref double current, ref double Power, ref bool done, int[] Straight)
        {
            if (current >= -1)
            {
                this.type = Power;
                for (int j = 0; j <= 12; j++)
                {
                    var fh = Straight.Where(o => o / 4 == j).ToArray();
                    if (fh.Length == 3 || done)
                    {
                        if (fh.Length == 2)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                current = 6;
                                Power = 13 * 2 + current * 100;
                                Win.Add(new Type() { Power = Power, Current = 6 });
                                sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                                break;
                            }
                            if (fh.Max() / 4 > 0)
                            {
                                current = 6;
                                Power = fh.Max() / 4 * 2 + current * 100;
                                Win.Add(new Type() { Power = Power, Current = 6 });
                                sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                                break;
                            }
                        }
                        if (!done)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                Power = 13;
                                done = true;
                                j = -1;
                            }
                            else
                            {
                                Power = fh.Max() / 4;
                                done = true;
                                j = -1;
                            }
                        }
                    }
                }
                if (current != 6)
                {
                    Power = this.type;
                }
            }
        }
        private void rFlush(ref double current, ref double Power, ref bool vf, int[] Straight1)
        {
            if (current >= -1)
            {
                var f1 = Straight1.Where(o => o % 4 == 0).ToArray();
                var f2 = Straight1.Where(o => o % 4 == 1).ToArray();
                var f3 = Straight1.Where(o => o % 4 == 2).ToArray();
                var f4 = Straight1.Where(o => o % 4 == 3).ToArray();
                if (f1.Length == 3 || f1.Length == 4)
                {
                    if (this.dealtCards[i] % 4 == this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f1[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f1.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        if (this.dealtCards[i + 1] / 4 > f1.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else if (this.dealtCards[i] / 4 < f1.Max() / 4 && this.dealtCards[i + 1] / 4 < f1.Max() / 4)
                        {
                            current = 5;
                            Power = f1.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f1.Length == 4)//different cards in hand
                {
                    if (this.dealtCards[i] % 4 != this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f1[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f1.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f1.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                    if (this.dealtCards[i + 1] % 4 != this.dealtCards[i] % 4 && this.dealtCards[i + 1] % 4 == f1[0] % 4)
                    {
                        if (this.dealtCards[i + 1] / 4 > f1.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f1.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f1.Length == 5)
                {
                    if (this.dealtCards[i] % 4 == f1[0] % 4 && this.dealtCards[i] / 4 > f1.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    if (this.dealtCards[i + 1] % 4 == f1[0] % 4 && this.dealtCards[i + 1] / 4 > f1.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i + 1] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    else if (this.dealtCards[i] / 4 < f1.Min() / 4 && this.dealtCards[i + 1] / 4 < f1.Min())
                    {
                        current = 5;
                        Power = f1.Max() + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                }

                if (f2.Length == 3 || f2.Length == 4)
                {
                    if (this.dealtCards[i] % 4 == this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f2[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f2.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        if (this.dealtCards[i + 1] / 4 > f2.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else if (this.dealtCards[i] / 4 < f2.Max() / 4 && this.dealtCards[i + 1] / 4 < f2.Max() / 4)
                        {
                            current = 5;
                            Power = f2.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f2.Length == 4)//different cards in hand
                {
                    if (this.dealtCards[i] % 4 != this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f2[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f2.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f2.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                    if (this.dealtCards[i + 1] % 4 != this.dealtCards[i] % 4 && this.dealtCards[i + 1] % 4 == f2[0] % 4)
                    {
                        if (this.dealtCards[i + 1] / 4 > f2.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f2.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f2.Length == 5)
                {
                    if (this.dealtCards[i] % 4 == f2[0] % 4 && this.dealtCards[i] / 4 > f2.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    if (this.dealtCards[i + 1] % 4 == f2[0] % 4 && this.dealtCards[i + 1] / 4 > f2.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i + 1] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    else if (this.dealtCards[i] / 4 < f2.Min() / 4 && this.dealtCards[i + 1] / 4 < f2.Min())
                    {
                        current = 5;
                        Power = f2.Max() + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                }

                if (f3.Length == 3 || f3.Length == 4)
                {
                    if (this.dealtCards[i] % 4 == this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f3[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f3.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        if (this.dealtCards[i + 1] / 4 > f3.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else if (this.dealtCards[i] / 4 < f3.Max() / 4 && this.dealtCards[i + 1] / 4 < f3.Max() / 4)
                        {
                            current = 5;
                            Power = f3.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f3.Length == 4)//different cards in hand
                {
                    if (this.dealtCards[i] % 4 != this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f3[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f3.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f3.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                    if (this.dealtCards[i + 1] % 4 != this.dealtCards[i] % 4 && this.dealtCards[i + 1] % 4 == f3[0] % 4)
                    {
                        if (this.dealtCards[i + 1] / 4 > f3.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f3.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f3.Length == 5)
                {
                    if (this.dealtCards[i] % 4 == f3[0] % 4 && this.dealtCards[i] / 4 > f3.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    if (this.dealtCards[i + 1] % 4 == f3[0] % 4 && this.dealtCards[i + 1] / 4 > f3.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i + 1] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    else if (this.dealtCards[i] / 4 < f3.Min() / 4 && this.dealtCards[i + 1] / 4 < f3.Min())
                    {
                        current = 5;
                        Power = f3.Max() + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                }

                if (f4.Length == 3 || f4.Length == 4)
                {
                    if (this.dealtCards[i] % 4 == this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f4[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f4.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        if (this.dealtCards[i + 1] / 4 > f4.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else if (this.dealtCards[i] / 4 < f4.Max() / 4 && this.dealtCards[i + 1] / 4 < f4.Max() / 4)
                        {
                            current = 5;
                            Power = f4.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f4.Length == 4)//different cards in hand
                {
                    if (this.dealtCards[i] % 4 != this.dealtCards[i + 1] % 4 && this.dealtCards[i] % 4 == f4[0] % 4)
                    {
                        if (this.dealtCards[i] / 4 > f4.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f4.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                    if (this.dealtCards[i + 1] % 4 != this.dealtCards[i] % 4 && this.dealtCards[i + 1] % 4 == f4[0] % 4)
                    {
                        if (this.dealtCards[i + 1] / 4 > f4.Max() / 4)
                        {
                            current = 5;
                            Power = this.dealtCards[i + 1] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                        else
                        {
                            current = 5;
                            Power = f4.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 5 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                            vf = true;
                        }
                    }
                }
                if (f4.Length == 5)
                {
                    if (this.dealtCards[i] % 4 == f4[0] % 4 && this.dealtCards[i] / 4 > f4.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    if (this.dealtCards[i + 1] % 4 == f4[0] % 4 && this.dealtCards[i + 1] / 4 > f4.Min() / 4)
                    {
                        current = 5;
                        Power = this.dealtCards[i + 1] + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                    else if (this.dealtCards[i] / 4 < f4.Min() / 4 && this.dealtCards[i + 1] / 4 < f4.Min())
                    {
                        current = 5;
                        Power = f4.Max() + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        vf = true;
                    }
                }
                //ace
                if (f1.Length > 0)
                {
                    if (this.dealtCards[i] / 4 == 0 && this.dealtCards[i] % 4 == f1[0] % 4 && vf && f1.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (this.dealtCards[i + 1] / 4 == 0 && this.dealtCards[i + 1] % 4 == f1[0] % 4 && vf && f1.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (f2.Length > 0)
                {
                    if (this.dealtCards[i] / 4 == 0 && this.dealtCards[i] % 4 == f2[0] % 4 && vf && f2.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (this.dealtCards[i + 1] / 4 == 0 && this.dealtCards[i + 1] % 4 == f2[0] % 4 && vf && f2.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (f3.Length > 0)
                {
                    if (this.dealtCards[i] / 4 == 0 && this.dealtCards[i] % 4 == f3[0] % 4 && vf && f3.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (this.dealtCards[i + 1] / 4 == 0 && this.dealtCards[i + 1] % 4 == f3[0] % 4 && vf && f3.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (f4.Length > 0)
                {
                    if (this.dealtCards[i] / 4 == 0 && this.dealtCards[i] % 4 == f4[0] % 4 && vf && f4.Length > 0)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (this.dealtCards[i + 1] / 4 == 0 && this.dealtCards[i + 1] % 4 == f4[0] % 4 && vf)
                    {
                        current = 5.5;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 5.5 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rStraight(ref double current, ref double Power, int[] Straight)
        {
            if (current >= -1)
            {
                var op = Straight.Select(o => o / 4).Distinct().ToArray();
                for (int j = 0; j < op.Length - 4; j++)
                {
                    if (op[j] + 4 == op[j + 4])
                    {
                        if (op.Max() - 4 == op[j])
                        {
                            current = 4;
                            Power = op.Max() + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 4 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                        else
                        {
                            current = 4;
                            Power = op[j + 4] + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 4 });
                            sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                    }
                    if (op[j] == 0 && op[j + 1] == 9 && op[j + 2] == 10 && op[j + 3] == 11 && op[j + 4] == 12)
                    {
                        current = 4;
                        Power = 13 + current * 100;
                        Win.Add(new Type() { Power = Power, Current = 4 });
                        sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rThreeOfAKind(ref double current, ref double Power, int[] Straight)
        {
            if (current >= -1)
            {
                for (int j = 0; j <= 12; j++)
                {
                    var fh = Straight.Where(o => o / 4 == j).ToArray();
                    if (fh.Length == 3)
                    {
                        if (fh.Max() / 4 == 0)
                        {
                            current = 3;
                            Power = 13 * 3 + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 3 });
                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                        else
                        {
                            current = 3;
                            Power = fh[0] / 4 + fh[1] / 4 + fh[2] / 4 + current * 100;
                            Win.Add(new Type() { Power = Power, Current = 3 });
                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                    }
                }
            }
        }
        private void rTwoPair(ref double current, ref double Power)
        {
            if (current >= -1)
            {
                bool msgbox = false;
                for (int tc = 16; tc >= 12; tc--)
                {
                    int max = tc - 12;
                    if (this.dealtCards[i] / 4 != this.dealtCards[i + 1] / 4)
                    {
                        for (int k = 1; k <= max; k++)
                        {
                            if (tc - k < 12)
                            {
                                max--;
                            }
                            if (tc - k >= 12)
                            {
                                if (this.dealtCards[i] / 4 == this.dealtCards[tc] / 4 && this.dealtCards[i + 1] / 4 == this.dealtCards[tc - k] / 4 ||
                                    this.dealtCards[i + 1] / 4 == this.dealtCards[tc] / 4 && this.dealtCards[i] / 4 == this.dealtCards[tc - k] / 4)
                                {
                                    if (!msgbox)
                                    {
                                        if (this.dealtCards[i] / 4 == 0)
                                        {
                                            current = 2;
                                            Power = 13 * 4 + (this.dealtCards[i + 1] / 4) * 2 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCards[i + 1] / 4 == 0)
                                        {
                                            current = 2;
                                            Power = 13 * 4 + (this.dealtCards[i] / 4) * 2 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCards[i + 1] / 4 != 0 && this.dealtCards[i] / 4 != 0)
                                        {
                                            current = 2;
                                            Power = (this.dealtCards[i] / 4) * 2 + (this.dealtCards[i + 1] / 4) * 2 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                    }
                                    msgbox = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void rPairTwoPair(ref double current, ref double Power)
        {
            if (current >= -1)
            {
                bool msgbox = false;
                bool msgbox1 = false;
                for (int tc = 16; tc >= 12; tc--)
                {
                    int max = tc - 12;
                    for (int k = 1; k <= max; k++)
                    {
                        if (tc - k < 12)
                        {
                            max--;
                        }
                        if (tc - k >= 12)
                        {
                            if (this.dealtCards[tc] / 4 == this.dealtCards[tc - k] / 4)
                            {
                                if (this.dealtCards[tc] / 4 != this.dealtCards[i] / 4 && this.dealtCards[tc] / 4 != this.dealtCards[i + 1] / 4 && current == 1)
                                {
                                    if (!msgbox)
                                    {
                                        if (this.dealtCards[i + 1] / 4 == 0)
                                        {
                                            current = 2;
                                            Power = (this.dealtCards[i] / 4) * 2 + 13 * 4 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCards[i] / 4 == 0)
                                        {
                                            current = 2;
                                            Power = (this.dealtCards[i + 1] / 4) * 2 + 13 * 4 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCards[i + 1] / 4 != 0)
                                        {
                                            current = 2;
                                            Power = (this.dealtCards[tc] / 4) * 2 + (this.dealtCards[i + 1] / 4) * 2 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCards[i] / 4 != 0)
                                        {
                                            current = 2;
                                            Power = (this.dealtCards[tc] / 4) * 2 + (this.dealtCards[i] / 4) * 2 + current * 100;
                                            Win.Add(new Type() { Power = Power, Current = 2 });
                                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                    }
                                    msgbox = true;
                                }
                                if (current == -1)
                                {
                                    if (!msgbox1)
                                    {
                                        if (this.dealtCards[i] / 4 > this.dealtCards[i + 1] / 4)
                                        {
                                            if (this.dealtCards[tc] / 4 == 0)
                                            {
                                                current = 0;
                                                Power = 13 + this.dealtCards[i] / 4 + current * 100;
                                                Win.Add(new Type() { Power = Power, Current = 1 });
                                                sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                current = 0;
                                                Power = this.dealtCards[tc] / 4 + this.dealtCards[i] / 4 + current * 100;
                                                Win.Add(new Type() { Power = Power, Current = 1 });
                                                sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                        }
                                        else
                                        {
                                            if (this.dealtCards[tc] / 4 == 0)
                                            {
                                                current = 0;
                                                Power = 13 + this.dealtCards[i + 1] + current * 100;
                                                Win.Add(new Type() { Power = Power, Current = 1 });
                                                sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                current = 0;
                                                Power = this.dealtCards[tc] / 4 + this.dealtCards[i + 1] / 4 + current * 100;
                                                Win.Add(new Type() { Power = Power, Current = 1 });
                                                sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                        }
                                    }
                                    msgbox1 = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void rPairFromHand(ref double current, ref double Power, int index)
        {
            if (current >= -1)
            {
                bool msgbox = false;
                if (this.dealtCards[index] / 4 == this.dealtCards[index + 1] / 4)
                {
                    if (!msgbox)
                    {
                        current = 1;
                        if (this.dealtCards[index] / 4 == 0)
                        {
                            Power = 13 * 4 + current * 100;
                        }
                        else
                        {
                            Power = (this.dealtCards[index + 1] / 4) * 4 + current * 100;
                        }

                        Win.Add(new Type() { Power = Power, Current = 1 });
                        sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                    }
                    msgbox = true;
                }
                for (int tc = 16; tc >= 12; tc--)
                {
                    if (this.dealtCards[index + 1] / 4 == this.dealtCards[tc] / 4)
                    {
                        if (!msgbox)
                        {
                            current = 1;

                            if (this.dealtCards[index + 1] / 4 == 0)
                            {
                                Power = 13 * 4 + this.dealtCards[index] / 4 + current * 100;
                            }
                            else
                            {
                                Power = (this.dealtCards[index + 1] / 4) * 4 + this.dealtCards[index] / 4 + current * 100;
                            }

                            Win.Add(new Type() { Power = Power, Current = 1 });
                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();

                        }
                        msgbox = true;
                    }
                    if (this.dealtCards[index] / 4 == this.dealtCards[tc] / 4)
                    {
                        if (!msgbox)
                        {
                            current = 1;

                            if (this.dealtCards[index] / 4 == 0)
                            {
                                Power = 13 * 4 + this.dealtCards[index + 1] / 4 + current * 100;
                            }
                            else
                            {
                                Power = (this.dealtCards[tc] / 4) * 4 + this.dealtCards[index + 1] / 4 + current * 100;
                            }

                            Win.Add(new Type() { Power = Power, Current = 1 });
                            sorted = Win.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                        msgbox = true;
                    }
                }
            }
        }
        private void rHighCard(ref double current, ref double Power)
        {
            if (current == -1)
            {
                if (this.dealtCards[i] / 4 > this.dealtCards[i + 1] / 4)
                {
                    current = -1;
                    Power = this.dealtCards[i] / 4;
                    Win.Add(new Type() { Power = Power, Current = -1 });
                    sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                }
                else
                {
                    current = -1;
                    Power = this.dealtCards[i + 1] / 4;
                    Win.Add(new Type() { Power = Power, Current = -1 });
                    sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                }
                if (this.dealtCards[i] / 4 == 0 || this.dealtCards[i + 1] / 4 == 0)
                {
                    current = -1;
                    Power = 13;
                    Win.Add(new Type() { Power = Power, Current = -1 });
                    sorted = Win.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                }
            }
        }

        void Winner(double current, double Power, string playerName, int chips, string lastly)
        {
            if (lastly == " ")
            {
                lastly = "Bot 5";
            }
            for (int j = 0; j <= 16; j++)
            {
                //await Task.Delay(5);
                if (Holder[j].Visible)
                    Holder[j].Image = Deck[j];
            }
            if (current == sorted.Current)
            {
                if (Power == sorted.Power)
                {

                    winnersCount++;
                    CheckWinners.Add(playerName);

                    if (current == -1)
                    {
                        MessageBox.Show(playerName + " High Card ");
                    }
                    if (current == 1 || current == 0)
                    {
                        MessageBox.Show(playerName + " Pair ");
                    }
                    if (current == 2)
                    {
                        MessageBox.Show(playerName + " Two Pair ");
                    }
                    if (current == 3)
                    {
                        MessageBox.Show(playerName + " Three of a Kind ");
                    }
                    if (current == 4)
                    {
                        MessageBox.Show(playerName + " Straight ");
                    }
                    if (current == 5 || current == 5.5)
                    {
                        MessageBox.Show(playerName + " Flush ");
                    }
                    if (current == 6)
                    {
                        MessageBox.Show(playerName + " Full House ");
                    }
                    if (current == 7)
                    {
                        MessageBox.Show(playerName + " Four of a Kind ");
                    }
                    if (current == 8)
                    {
                        MessageBox.Show(playerName + " Straight Flush ");
                    }
                    if (current == 9)
                    {
                        MessageBox.Show(playerName + " Royal Flush ! ");
                    }
                }
            }
            if (playerName == lastly)//lastfixed
            {
                if (winnersCount > 1)
                {
                    if (CheckWinners.Contains("Player"))
                    {
                        this.playerChips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxPlayerChips.Text = this.playerChips.ToString();
                        //pPanel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        bot1Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot1Chips.Text = bot1Chips.ToString();
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        bot2Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot2Chips.Text = bot2Chips.ToString();
                        //b2Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        bot3Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot3Chips.Text = bot3Chips.ToString();
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        bot4Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot4Chips.Text = bot4Chips.ToString();
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        bot5Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot5Chips.Text = bot5Chips.ToString();
                        //b5Panel.Visible = true;
                    }
                    //await Finish(1);
                }
                if (winnersCount == 1)
                {
                    if (CheckWinners.Contains("Player"))
                    {
                        this.playerChips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //pPanel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        bot1Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        bot2Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b2Panel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        bot3Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        bot4Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        bot5Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b5Panel.Visible = true;
                    }
                }
            }
        }
        async Task CheckRaise(int currentTurn, int raiseTurn)
        {
            if (raising)
            {
                turnCount = 0;
                raising = false;
                raisedTurn = currentTurn;
                changed = true;
            }
            else
            {
                if (turnCount >= maxPlayersLeftCount - 1 || !changed && turnCount == maxPlayersLeftCount)
                {
                    if (currentTurn == raisedTurn - 1 || !changed && turnCount == maxPlayersLeftCount || raisedTurn == 0 && currentTurn == 5)
                    {
                        changed = false;
                        turnCount = 0;
                        this.raise = 0;
                        this.pokerCall = 0;
                        raisedTurn = 123;
                        rounds++;
                        if (!this.playerGameEnded)
                            labelPlayerStatus.Text = "";
                        if (!this.bot1GameEnded)
                            labelBot1Status.Text = "";
                        if (!this.bot2GameEnded)
                            labelBot2Status.Text = "";
                        if (!this.bot3GameEnded)
                            labelBot3Status.Text = "";
                        if (!this.bot4GameEnded)
                            labelBot4Status.Text = "";
                        if (!this.bot5GameEnded)
                            labelBot5Status.Text = "";
                    }
                }
            }
            if (rounds == Flop)
            {
                for (int j = 12; j <= 14; j++)
                {
                    if (Holder[j].Image != Deck[j])
                    {
                        Holder[j].Image = Deck[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == Turn)
            {
                for (int j = 14; j <= 15; j++)
                {
                    if (Holder[j].Image != Deck[j])
                    {
                        Holder[j].Image = Deck[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == River)
            {
                for (int j = 15; j <= 16; j++)
                {
                    if (Holder[j].Image != Deck[j])
                    {
                        Holder[j].Image = Deck[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == End && maxPlayersLeftCount == 6)
            {
                string fixedLast = "qwerty";
                if (!labelPlayerStatus.Text.Contains("Fold"))
                {
                    fixedLast = "Player";
                    Rules(0, 1, "Player", ref this.playerType, ref this.playerPower, this.playerGameEnded);
                }
                if (!labelBot1Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 1";
                    Rules(2, 3, "Bot 1", ref this.bot1Type, ref this.bot1Power, this.bot1GameEnded);
                }
                if (!labelBot2Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 2";
                    Rules(4, 5, "Bot 2", ref this.bot2Type, ref this.bot2Power,this.bot2GameEnded);
                }
                if (!labelBot3Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 3";
                    Rules(6, 7, "Bot 3", ref this.bot3Type, ref this.bot3Power, this.bot3GameEnded);
                }
                if (!labelBot4Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 4";
                    Rules(8, 9, "Bot 4", ref this.bot4Type, ref this.bot4Power, this.bot4GameEnded);
                }
                if (!labelBot5Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 5";
                    Rules(10, 11, "Bot 5", ref this.bot5Type, ref this.bot5Power, this.bot5GameEnded);
                }
                Winner(this.playerType, this.playerPower, "Player", this.playerChips, fixedLast);
                Winner(this.bot1Type, this.bot1Power, "Bot 1", bot1Chips, fixedLast);
                Winner(this.bot2Type, this.bot2Power, "Bot 2", bot2Chips, fixedLast);
                Winner(this.bot3Type, this.bot3Power, "Bot 3", bot3Chips, fixedLast);
                Winner(this.bot4Type, this.bot4Power, "Bot 4", bot4Chips, fixedLast);
                Winner(this.bot5Type, this.bot5Power, "Bot 5", bot5Chips, fixedLast);
                restart = true;
                Pturn = true;
                this.playerGameEnded = false;
                this.bot1GameEnded = false;
                this.bot2GameEnded = false;
                this.bot3GameEnded = false;
                this.bot4GameEnded = false;
                this.bot5GameEnded = false;
                if (this.playerChips <= 0)
                {
                    AddChips f2 = new AddChips();
                    f2.ShowDialog();
                    if (f2.a != 0)
                    {
                        this.playerChips = f2.a;
                        bot1Chips += f2.a;
                        bot2Chips += f2.a;
                        bot3Chips += f2.a;
                        bot4Chips += f2.a;
                        bot5Chips += f2.a;
                        this.playerGameEnded = false;
                        Pturn = true;
                        buttonRaise.Enabled = true;
                        buttonFold.Enabled = true;
                        buttonCheck.Enabled = true;
                        buttonRaise.Text = "Raise";
                    }
                }
                this.playerCardsPanel.Visible = false; this.bot1CardsPanel.Visible = false; this.bot2CardsPanel.Visible = false; this.bot3CardsPanel.Visible = false; this.bot4CardsPanel.Visible = false; this.bot5CardsPanel.Visible = false;
                this.playerCall = 0; playerRaise = 0;
                this.bot1Call = 0; bot1Raise = 0;
                this.bot2Call = 0; bot2Raise = 0;
                bot3Call = 0; bot3Raise = 0;
                bot4Call = 0; bot4Raise = 0;
                bot5Call = 0; bot5Raise = 0;
                lastBotPlayed = 0;
                this.pokerCall = bb;
                this.raise = 0;
                ImgLocation = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
                bools.Clear();
                rounds = 0;
                this.playerPower = 0; this.playerType = -1;
                type = 0; this.bot1Power = 0; this.bot2Power = 0; this.bot3Power = 0; this.bot4Power = 0; this.bot5Power = 0;
                this.bot1Type = -1; this.bot2Type = -1; this.bot3Type = -1; this.bot4Type = -1; this.bot5Type = -1;
                ints.Clear();
                CheckWinners.Clear();
                winnersCount = 0;
                Win.Clear();
                sorted.Current = 0;
                sorted.Power = 0;
                for (int os = 0; os < 17; os++)
                {
                    Holder[os].Image = null;
                    Holder[os].Invalidate();
                    Holder[os].Visible = false;
                }
                textBoxPot.Text = "0";
                labelPlayerStatus.Text = "";
                await Shuffle();
                await Turns();
            }
        }
        void FixCall(Label status, ref int cCall, ref int cRaise, int options)
        {
            if (rounds != 4)
            {
                if (options == 1)
                {
                    if (status.Text.Contains("Raise"))
                    {
                        var changeRaise = status.Text.Substring(6);
                        cRaise = int.Parse(changeRaise);
                    }
                    if (status.Text.Contains("Call"))
                    {
                        var changeCall = status.Text.Substring(5);
                        cCall = int.Parse(changeCall);
                    }
                    if (status.Text.Contains("Check"))
                    {
                        cRaise = 0;
                        cCall = 0;
                    }
                }
                if (options == 2)
                {
                    if (cRaise != this.raise && cRaise <= this.raise)
                    {
                        this.pokerCall = Convert.ToInt32(this.raise) - cRaise;
                    }
                    if (cCall != this.pokerCall || cCall <= this.pokerCall)
                    {
                        this.pokerCall = this.pokerCall - cCall;
                    }
                    if (cRaise == this.raise && this.raise > 0)
                    {
                        this.pokerCall = 0;
                        buttonCall.Enabled = false;
                        buttonCall.Text = "Callisfuckedup";
                    }
                }
            }
        }
        async Task AllIn()
        {
            #region All in
            if (this.playerChips <= 0 && !intsadded)
            {
                if (labelPlayerStatus.Text.Contains("Raise"))
                {
                    ints.Add(this.playerChips);
                    intsadded = true;
                }
                if (labelPlayerStatus.Text.Contains("Call"))
                {
                    ints.Add(this.playerChips);
                    intsadded = true;
                }
            }
            intsadded = false;
            if (bot1Chips <= 0 && !this.bot1GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot1Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot2Chips <= 0 && !this.bot2GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot2Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot3Chips <= 0 && !this.bot3GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot3Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot4Chips <= 0 && !this.bot4GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot4Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot5Chips <= 0 && !this.bot5GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot5Chips);
                    intsadded = true;
                }
            }
            if (ints.ToArray().Length == this.maxPlayersLeftCount)
            {
                await Finish(2);
            }
            else
            {
                ints.Clear();
            }
            #endregion

            var abc = bools.Count(x => x == false);

            #region LastManStanding
            if (abc == 1)
            {
                int index = bools.IndexOf(false);
                if (index == 0)
                {
                    this.playerChips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.playerChips.ToString();
                    this.playerCardsPanel.Visible = true;
                    MessageBox.Show("Player Wins");
                }
                if (index == 1)
                {
                    bot1Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot1Chips.ToString();
                    this.bot1CardsPanel.Visible = true;
                    MessageBox.Show("Bot 1 Wins");
                }
                if (index == 2)
                {
                    bot2Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot2Chips.ToString();
                    this.bot2CardsPanel.Visible = true;
                    MessageBox.Show("Bot 2 Wins");
                }
                if (index == 3)
                {
                    bot3Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot3Chips.ToString();
                    this.bot3CardsPanel.Visible = true;
                    MessageBox.Show("Bot 3 Wins");
                }
                if (index == 4)
                {
                    bot4Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot4Chips.ToString();
                    this.bot4CardsPanel.Visible = true;
                    MessageBox.Show("Bot 4 Wins");
                }
                if (index == 5)
                {
                    bot5Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot5Chips.ToString();
                    this.bot5CardsPanel.Visible = true;
                    MessageBox.Show("Bot 5 Wins");
                }
                for (int j = 0; j <= 16; j++)
                {
                    Holder[j].Visible = false;
                }
                await Finish(1);
            }
            intsadded = false;
            #endregion

            #region FiveOrLessLeft
            if (abc < 6 && abc > 1 && rounds >= End)
            {
                await Finish(2);
            }
            #endregion


        }
        async Task Finish(int n)
        {
            if (n == 2)
            {
                FixWinners();
            }
            this.playerCardsPanel.Visible = false; this.bot1CardsPanel.Visible = false; this.bot2CardsPanel.Visible = false; this.bot3CardsPanel.Visible = false; this.bot4CardsPanel.Visible = false; this.bot5CardsPanel.Visible = false;
            this.pokerCall = bb; this.raise = 0;
            foldedPlayers = 5;
            type = 0; rounds = 0; this.bot1Power = 0; this.bot2Power = 0; this.bot3Power = 0; this.bot4Power = 0; this.bot5Power = 0; this.playerPower = 0; this.playerType = -1; this.raise = 0;
            this.bot1Type = -1; this.bot2Type = -1; this.bot3Type = -1; this.bot4Type = -1; this.bot5Type = -1;
            this.bot1Turn = false; this.bot2Turn = false; this.bot3Turn = false; this.bot4Turn = false; this.bot5Turn = false;
            this.bot1GameEnded = false; this.bot2GameEnded = false; this.bot3GameEnded = false; this.bot4GameEnded = false; this.bot5GameEnded = false;
            this.playerFolded = false; this.bot1Folded = false; this.bot2Folded = false; this.bot3Folded = false; this.bot4Folded = false; this.bot5Folded = false;
            this.playerGameEnded = false; Pturn = true; restart = false; raising = false;
            this.playerCall = 0; this.bot1Call = 0; this.bot2Call = 0; bot3Call = 0; bot4Call = 0; bot5Call = 0; playerRaise = 0; bot1Raise = 0; bot2Raise = 0; bot3Raise = 0; bot4Raise = 0; bot5Raise = 0;
            height = 0; width = 0; winnersCount = 0; Flop = 1; Turn = 2; River = 3; End = 4; maxPlayersLeftCount = 6;
            lastBotPlayed = 123; raisedTurn = 1;
            bools.Clear();
            CheckWinners.Clear();
            ints.Clear();
            Win.Clear();
            sorted.Current = 0;
            sorted.Power = 0;
            textBoxPot.Text = "0";
            t = 60; up = 10000000; turnCount = 0;
            labelPlayerStatus.Text = "";
            labelBot1Status.Text = "";
            labelBot2Status.Text = "";
            labelBot3Status.Text = "";
            labelBot4Status.Text = "";
            labelBot5Status.Text = "";
            if (this.playerChips <= 0)
            {
                AddChips f2 = new AddChips();
                f2.ShowDialog();
                if (f2.a != 0)
                {
                    this.playerChips = f2.a;
                    bot1Chips += f2.a;
                    bot2Chips += f2.a;
                    bot3Chips += f2.a;
                    bot4Chips += f2.a;
                    bot5Chips += f2.a;
                    this.playerGameEnded = false;
                    Pturn = true;
                    buttonRaise.Enabled = true;
                    buttonFold.Enabled = true;
                    buttonCheck.Enabled = true;
                    buttonRaise.Text = "Raise";
                }
            }
            ImgLocation = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
            for (int os = 0; os < 17; os++)
            {
                Holder[os].Image = null;
                Holder[os].Invalidate();
                Holder[os].Visible = false;
            }
            await Shuffle();
            //await Turns();
        }
        void FixWinners()
        {
            Win.Clear();
            sorted.Current = 0;
            sorted.Power = 0;
            string fixedLast = "qwerty";
            if (!labelPlayerStatus.Text.Contains("Fold"))
            {
                fixedLast = "Player";
                Rules(0, 1, "Player", ref this.playerType, ref this.playerPower, this.playerGameEnded);
            }
            if (!labelBot1Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 1";
                Rules(2, 3, "Bot 1", ref this.bot1Type, ref this.bot1Power, this.bot1GameEnded);
            }
            if (!labelBot2Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 2";
                Rules(4, 5, "Bot 2", ref this.bot2Type, ref this.bot2Power, bot2GameEnded);
            }
            if (!labelBot3Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 3";
                Rules(6, 7, "Bot 3", ref this.bot3Type, ref this.bot3Power, bot3GameEnded);
            }
            if (!labelBot4Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 4";
                Rules(8, 9, "Bot 4", ref this.bot4Type, ref this.bot4Power, bot4GameEnded);
            }
            if (!labelBot5Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 5";
                Rules(10, 11, "Bot 5", ref this.bot5Type, ref this.bot5Power, bot5GameEnded);
            }
            Winner(this.playerType, this.playerPower, "Player", this.playerChips, fixedLast);
            Winner(this.bot1Type, this.bot1Power, "Bot 1", bot1Chips, fixedLast);
            Winner(this.bot2Type, this.bot2Power, "Bot 2", bot2Chips, fixedLast);
            Winner(this.bot3Type, this.bot3Power, "Bot 3", bot3Chips, fixedLast);
            Winner(this.bot4Type, this.bot4Power, "Bot 4", bot4Chips, fixedLast);
            Winner(this.bot5Type, this.bot5Power, "Bot 5", bot5Chips, fixedLast);
        }
        void AI(int c1, int c2, ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower, double botCurrent)
        {
            if (!sFTurn)
            {
                if (botCurrent == -1)
                {
                    HighCard(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower);
                }
                if (botCurrent == 0)
                {
                    PairTable(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower);
                }
                if (botCurrent == 1)
                {
                    PairHand(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower);
                }
                if (botCurrent == 2)
                {
                    TwoPair(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower);
                }
                if (botCurrent == 3)
                {
                    ThreeOfAKind(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
                if (botCurrent == 4)
                {
                    Straight(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
                if (botCurrent == 5 || botCurrent == 5.5)
                {
                    Flush(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
                if (botCurrent == 6)
                {
                    FullHouse(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
                if (botCurrent == 7)
                {
                    FourOfAKind(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
                if (botCurrent == 8 || botCurrent == 9)
                {
                    StraightFlush(ref sChips, ref sTurn, ref sFTurn, sStatus, name, botPower);
                }
            }
            if (sFTurn)
            {
                Holder[c1].Visible = false;
                Holder[c2].Visible = false;
            }
        }
        private void HighCard(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            HP(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower, 20, 25);
        }
        private void PairTable(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            HP(ref sChips, ref sTurn, ref sFTurn, sStatus, botPower, 16, 25);
        }
        private void PairHand(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(10, 16);
            int rRaise = rPair.Next(10, 13);
            if (botPower <= 199 && botPower >= 140)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 6, rRaise);
            }
            if (botPower <= 139 && botPower >= 128)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 7, rRaise);
            }
            if (botPower < 128 && botPower >= 101)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 9, rRaise);
            }
        }
        private void TwoPair(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(6, 11);
            int rRaise = rPair.Next(6, 11);
            if (botPower <= 290 && botPower >= 246)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 3, rRaise);
            }
            if (botPower <= 244 && botPower >= 234)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 4, rRaise);
            }
            if (botPower < 234 && botPower >= 201)
            {
                PH(ref sChips, ref sTurn, ref sFTurn, sStatus, rCall, 4, rRaise);
            }
        }
        private void ThreeOfAKind(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random tk = new Random();
            int tCall = tk.Next(3, 7);
            int tRaise = tk.Next(4, 8);
            if (botPower <= 390 && botPower >= 330)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
            if (botPower <= 327 && botPower >= 321)//10  8
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
            if (botPower < 321 && botPower >= 303)//7 2
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
        }
        private void Straight(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random str = new Random();
            int sCall = str.Next(3, 6);
            int sRaise = str.Next(3, 8);
            if (botPower <= 480 && botPower >= 410)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
            if (botPower <= 409 && botPower >= 407)//10  8
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
            if (botPower < 407 && botPower >= 404)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
        }
        private void Flush(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random fsh = new Random();
            int fCall = fsh.Next(2, 6);
            int fRaise = fsh.Next(3, 7);
            Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, fCall, fRaise);
        }
        private void FullHouse(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random flh = new Random();
            int fhCall = flh.Next(1, 5);
            int fhRaise = flh.Next(2, 6);
            if (botPower <= 626 && botPower >= 620)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, fhCall, fhRaise);
            }
            if (botPower < 620 && botPower >= 602)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, fhCall, fhRaise);
            }
        }
        private void FourOfAKind(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random fk = new Random();
            int fkCall = fk.Next(1, 4);
            int fkRaise = fk.Next(2, 5);
            if (botPower <= 752 && botPower >= 704)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, fkCall, fkRaise);
            }
        }
        private void StraightFlush(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random sf = new Random();
            int sfCall = sf.Next(1, 3);
            int sfRaise = sf.Next(1, 3);
            if (botPower <= 913 && botPower >= 804)
            {
                Smooth(ref sChips, ref sTurn, ref sFTurn, sStatus, name, sfCall, sfRaise);
            }
        }

        private void Fold(ref bool sTurn, ref bool sFTurn, Label sStatus)
        {
            raising = false;
            sStatus.Text = "Fold";
            sTurn = false;
            sFTurn = true;
        }
        private void Check(ref bool cTurn, Label cStatus)
        {
            cStatus.Text = "Check";
            cTurn = false;
            raising = false;
        }
        private void Call(ref int sChips, ref bool sTurn, Label sStatus)
        {
            raising = false;
            sTurn = false;
            sChips -= this.pokerCall;
            sStatus.Text = "Call " + this.pokerCall;
            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
        }
        private void Raised(ref int sChips, ref bool sTurn, Label sStatus)
        {
            sChips -= Convert.ToInt32(this.raise);
            sStatus.Text = "Raise " + this.raise;
            textBoxPot.Text = (int.Parse(textBoxPot.Text) + Convert.ToInt32(this.raise)).ToString();
            this.pokerCall = Convert.ToInt32(this.raise);
            raising = true;
            sTurn = false;
        }
        private static double RoundN(int sChips, int n)
        {
            double a = Math.Round((sChips / n) / 100d, 0) * 100;
            return a;
        }
        private void HP(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower, int n, int n1)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 4);
            if (this.pokerCall <= 0)
            {
                Check(ref sTurn, sStatus);
            }
            if (this.pokerCall > 0)
            {
                if (rnd == 1)
                {
                    if (this.pokerCall <= RoundN(sChips, n))
                    {
                        Call(ref sChips, ref sTurn, sStatus);
                    }
                    else
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                }
                if (rnd == 2)
                {
                    if (this.pokerCall <= RoundN(sChips, n1))
                    {
                        Call(ref sChips, ref sTurn, sStatus);
                    }
                    else
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                }
            }
            if (rnd == 3)
            {
                if (this.raise == 0)
                {
                    this.raise = this.pokerCall * 2;
                    Raised(ref sChips, ref sTurn, sStatus);
                }
                else
                {
                    if (this.raise <= RoundN(sChips, n))
                    {
                        this.raise = this.pokerCall * 2;
                        Raised(ref sChips, ref sTurn, sStatus);
                    }
                    else
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                }
            }
            if (sChips <= 0)
            {
                sFTurn = true;
            }
        }
        private void PH(ref int sChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int n, int n1, int r)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 3);
            if (rounds < 2)
            {
                if (this.pokerCall <= 0)
                {
                    Check(ref sTurn, sStatus);
                }
                if (this.pokerCall > 0)
                {
                    if (this.pokerCall >= RoundN(sChips, n1))
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                    if (this.raise > RoundN(sChips, n))
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                    if (!sFTurn)
                    {
                        if (this.pokerCall >= RoundN(sChips, n) && this.pokerCall <= RoundN(sChips, n1))
                        {
                            Call(ref sChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= RoundN(sChips, n) && this.raise >= (RoundN(sChips, n)) / 2)
                        {
                            Call(ref sChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= (RoundN(sChips, n)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = RoundN(sChips, n);
                                Raised(ref sChips, ref sTurn, sStatus);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                Raised(ref sChips, ref sTurn, sStatus);
                            }
                        }

                    }
                }
            }
            if (rounds >= 2)
            {
                if (this.pokerCall > 0)
                {
                    if (this.pokerCall >= RoundN(sChips, n1 - rnd))
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                    if (this.raise > RoundN(sChips, n - rnd))
                    {
                        Fold(ref sTurn, ref sFTurn, sStatus);
                    }
                    if (!sFTurn)
                    {
                        if (this.pokerCall >= RoundN(sChips, n - rnd) && this.pokerCall <= RoundN(sChips, n1 - rnd))
                        {
                            Call(ref sChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= RoundN(sChips, n - rnd) && this.raise >= (RoundN(sChips, n - rnd)) / 2)
                        {
                            Call(ref sChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= (RoundN(sChips, n - rnd)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = RoundN(sChips, n - rnd);
                                Raised(ref sChips, ref sTurn, sStatus);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                Raised(ref sChips, ref sTurn, sStatus);
                            }
                        }
                    }
                }
                if (this.pokerCall <= 0)
                {
                    this.raise = RoundN(sChips, r - rnd);
                    Raised(ref sChips, ref sTurn, sStatus);
                }
            }
            if (sChips <= 0)
            {
                sFTurn = true;
            }
        }
        void Smooth(ref int botChips, ref bool botTurn, ref bool botFTurn, Label botStatus, int name, int n, int r)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 3);
            if (this.pokerCall <= 0)
            {
                Check(ref botTurn, botStatus);
            }
            else
            {
                if (this.pokerCall >= RoundN(botChips, n))
                {
                    if (botChips > this.pokerCall)
                    {
                        Call(ref botChips, ref botTurn, botStatus);
                    }
                    else if (botChips <= this.pokerCall)
                    {
                        raising = false;
                        botTurn = false;
                        botChips = 0;
                        botStatus.Text = "Call " + botChips;
                        textBoxPot.Text = (int.Parse(textBoxPot.Text) + botChips).ToString();
                    }
                }
                else
                {
                    if (this.raise > 0)
                    {
                        if (botChips >= this.raise * 2)
                        {
                            this.raise *= 2;
                            Raised(ref botChips, ref botTurn, botStatus);
                        }
                        else
                        {
                            Call(ref botChips, ref botTurn, botStatus);
                        }
                    }
                    else
                    {
                        this.raise = this.pokerCall * 2;
                        Raised(ref botChips, ref botTurn, botStatus);
                    }
                }
            }
            if (botChips <= 0)
            {
                botFTurn = true;
            }
        }

        #region UI
        private async void timer_Tick(object sender, object e)
        {
            if (progressBarTimer.Value <= 0)
            {
                this.playerGameEnded = true;
                await Turns();
            }
            if (t > 0)
            {
                t--;
                progressBarTimer.Value = (t / 6) * 100;
            }
        }
        private void Update_Tick(object sender, object e)
        {
            if (this.playerChips <= 0)
            {
                textBoxPlayerChips.Text = "Chips : 0";
            }
            if (bot1Chips <= 0)
            {
                textBoxBot1Chips.Text = "Chips : 0";
            }
            if (bot2Chips <= 0)
            {
                textBoxBot2Chips.Text = "Chips : 0";
            }
            if (bot3Chips <= 0)
            {
                textBoxBot3Chips.Text = "Chips : 0";
            }
            if (bot4Chips <= 0)
            {
                textBoxBot4Chips.Text = "Chips : 0";
            }
            if (bot5Chips <= 0)
            {
                textBoxBot5Chips.Text = "Chips : 0";
            }
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
            textBoxBot1Chips.Text = "Chips : " + bot1Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + bot2Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + bot3Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + bot4Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + bot5Chips.ToString();
            if (this.playerChips <= 0)
            {
                Pturn = false;
                this.playerGameEnded = true;
                buttonCall.Enabled = false;
                buttonRaise.Enabled = false;
                buttonFold.Enabled = false;
                buttonCheck.Enabled = false;
            }
            if (up > 0)
            {
                up--;
            }
            if (this.playerChips >= this.pokerCall)
            {
                buttonCall.Text = "Call " + this.pokerCall.ToString();
            }
            else
            {
                buttonCall.Text = "All in";
                buttonRaise.Enabled = false;
            }
            if (this.pokerCall > 0)
            {
                buttonCheck.Enabled = false;
            }
            if (this.pokerCall <= 0)
            {
                buttonCheck.Enabled = true;
                buttonCall.Text = "Call";
                buttonCall.Enabled = false;
            }
            if (this.playerChips <= 0)
            {
                buttonRaise.Enabled = false;
            }
            int parsedValue;

            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.playerChips <= int.Parse(textBoxRaise.Text))
                {
                    buttonRaise.Text = "All in";
                }
                else
                {
                    buttonRaise.Text = "Raise";
                }
            }
            if (this.playerChips < this.pokerCall)
            {
                buttonRaise.Enabled = false;
            }
        }
        private async void ButtonFold_Click(object sender, EventArgs e)
        {

        }
        private async void ButtonCheck_Click(object sender, EventArgs e)
        {
            if (this.pokerCall <= 0)
            {
                Pturn = false;
                labelPlayerStatus.Text = "Check";
            }
            else
            {
                //pStatus.Text = "All in " + Chips;

                buttonCheck.Enabled = false;
            }
            await Turns();
        }
        private async void ButtonCall_Click(object sender, EventArgs e)
        {
            Rules(0, 1, "Player", ref this.playerType, ref this.playerPower, this.playerGameEnded);

            if (this.playerChips >= this.pokerCall)
            {
                this.playerChips -= this.pokerCall;
                textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();

                if (textBoxPot.Text != "")
                {
                    textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                }
                else
                {
                    textBoxPot.Text = this.pokerCall.ToString();
                }

                Pturn = false;
                labelPlayerStatus.Text = "Call " + this.pokerCall;
                this.playerCall = this.pokerCall;
            }
            else if (this.playerChips <= this.pokerCall && this.pokerCall > 0)
            {
                textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.playerChips).ToString();
                labelPlayerStatus.Text = "All in " + this.playerChips;
                this.playerChips = 0;
                textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
                Pturn = false;
                buttonFold.Enabled = false;
                this.playerCall = this.playerChips;
            }
            await Turns();
        }
        private async void ButtonRaise_Click(object sender, EventArgs e)
        {
            Rules(0, 1, "Player", ref this.playerType, ref this.playerPower, this.playerGameEnded);
            int parsedValue;
            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.playerChips > this.pokerCall)
                {
                    if (this.raise * 2 > int.Parse(textBoxRaise.Text))
                    {
                        textBoxRaise.Text = (this.raise * 2).ToString();
                        MessageBox.Show("You must raise atleast twice as the current raise !");
                        return;
                    }
                    else
                    {
                        if (this.playerChips >= int.Parse(textBoxRaise.Text))
                        {
                            this.pokerCall = int.Parse(textBoxRaise.Text);
                            this.raise = int.Parse(textBoxRaise.Text);
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                            buttonCall.Text = "Call";
                            this.playerChips -= int.Parse(textBoxRaise.Text);
                            raising = true;
                            lastBotPlayed = 0;
                            playerRaise = Convert.ToInt32(this.raise);
                        }
                        else
                        {
                            this.pokerCall = this.playerChips;
                            this.raise = this.playerChips;
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.playerChips).ToString();
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            this.playerChips = 0;
                            raising = true;
                            lastBotPlayed = 0;
                            playerRaise = Convert.ToInt32(this.raise);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("This is a number only field");
                return;
            }
            Pturn = false;
            await Turns();
        }
        private void ButtonAddChips_Click(object sender, EventArgs e)
        {
            if (textBoxAddChips.Text == "") { }
            else
            {
                this.playerChips += int.Parse(textBoxAddChips.Text);
                bot1Chips += int.Parse(textBoxAddChips.Text);
                bot2Chips += int.Parse(textBoxAddChips.Text);
                bot3Chips += int.Parse(textBoxAddChips.Text);
                bot4Chips += int.Parse(textBoxAddChips.Text);
                bot5Chips += int.Parse(textBoxAddChips.Text);
            }
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
        }
        private void ButtonChooseBlind_Click(object sender, EventArgs e)
        {
            textBoxBigBlind.Text = bb.ToString();
            textBoxSmallBlind.Text = sb.ToString();
            if (textBoxBigBlind.Visible == false)
            {
                textBoxBigBlind.Visible = true;
                textBoxSmallBlind.Visible = true;
                buttonBigBlind.Visible = true;
                buttonSmallBlind.Visible = true;
            }
            else
            {
                textBoxBigBlind.Visible = false;
                textBoxSmallBlind.Visible = false;
                buttonBigBlind.Visible = false;
                buttonSmallBlind.Visible = false;
            }
        }
        private void ButtonSmallBlind_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (textBoxSmallBlind.Text.Contains(",") || textBoxSmallBlind.Text.Contains("."))
            {
                MessageBox.Show("The Small Blind can be only round number !");
                textBoxSmallBlind.Text = sb.ToString();
                return;
            }
            if (!int.TryParse(textBoxSmallBlind.Text, out parsedValue))
            {
                MessageBox.Show("This is a number only field");
                textBoxSmallBlind.Text = sb.ToString();
                return;
            }
            if (int.Parse(textBoxSmallBlind.Text) > 100000)
            {
                MessageBox.Show("The maximum of the Small Blind is 100 000 $");
                textBoxSmallBlind.Text = sb.ToString();
            }
            if (int.Parse(textBoxSmallBlind.Text) < 250)
            {
                MessageBox.Show("The minimum of the Small Blind is 250 $");
            }
            if (int.Parse(textBoxSmallBlind.Text) >= 250 && int.Parse(textBoxSmallBlind.Text) <= 100000)
            {
                sb = int.Parse(textBoxSmallBlind.Text);
                MessageBox.Show("The changes have been saved ! They will become available the next hand you play. ");
            }
        }
        private void ButtonBigBlind_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (textBoxBigBlind.Text.Contains(",") || textBoxBigBlind.Text.Contains("."))
            {
                MessageBox.Show("The Big Blind can be only round number !");
                textBoxBigBlind.Text = bb.ToString();
                return;
            }
            if (!int.TryParse(textBoxSmallBlind.Text, out parsedValue))
            {
                MessageBox.Show("This is a number only field");
                textBoxSmallBlind.Text = bb.ToString();
                return;
            }
            if (int.Parse(textBoxBigBlind.Text) > 200000)
            {
                MessageBox.Show("The maximum of the Big Blind is 200 000");
                textBoxBigBlind.Text = bb.ToString();
            }
            if (int.Parse(textBoxBigBlind.Text) < 500)
            {
                MessageBox.Show("The minimum of the Big Blind is 500 $");
            }
            if (int.Parse(textBoxBigBlind.Text) >= 500 && int.Parse(textBoxBigBlind.Text) <= 200000)
            {
                bb = int.Parse(textBoxBigBlind.Text);
                MessageBox.Show("The changes have been saved ! They will become available the next hand you play. ");
            }
        }
        private void Layout_Change(object sender, LayoutEventArgs e)
        {
            width = this.Width;
            height = this.Height;
        }
        #endregion
    }
}