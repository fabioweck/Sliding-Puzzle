using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;

/*
    SODV2101 23SEPMNRT5 : Rapid Application Development
    Fabio Augusto Weck
    #441977

    *** References - http://www.csharphelper.com/howtos/howto_randomize_2d_array.html ***

 */

namespace Sliding_puzzle
{
    public partial class Form1 : Form
    {
        public Label[,] Labels { get; set; }
        public Label[] AllLabels { get; set; }
        public int Moves { get; set; } = 0;
        public static string Difficulty { get; set; }
        public Form intro { get; set; }

        public Form1()
        {

            InitializeComponent();

            Intro();
            SetForm();
            SetIconsOnScreen();
            DefineArray();
            ShuffleArray(Labels);
            RenderNumbers();
        }

        private void Intro()
        {
            intro = new();
            intro.Size = new Size(350, 300);
            intro.Text = "Sliding puzzle!";
            intro.StartPosition = FormStartPosition.CenterScreen;

            Label selectDifficulty = new();
            selectDifficulty.AutoSize = true;
            selectDifficulty.Font = new Font("Tw Cen MT Condensed Extra Bold", 12);
            selectDifficulty.Location = new Point(150 - (selectDifficulty.Width / 2), 15);
            selectDifficulty.Text = "Select difficulty";

            RadioButton selectEasy = new();
            selectEasy.Location = new Point(125, 60);
            selectEasy.Text = "Easy";
            selectEasy.Checked = true;

            RadioButton selectMedium = new();
            selectMedium.Text = "Medium";
            selectMedium.Location = new Point(125, 100);

            RadioButton selectHard = new();
            selectHard.Text = "Hard";
            selectHard.Location = new Point(125, 140);


            Button start = new();
            start.Size = new Size(50, 30);
            start.Location = new Point(130, 200);
            start.Text = "Start";
            start.Click += Start;

            intro.Controls.Add(selectDifficulty);
            intro.Controls.Add(selectEasy);
            intro.Controls.Add(selectMedium);
            intro.Controls.Add(selectHard);
            intro.Controls.Add(start);

            CenterToScreen();
            intro.ShowDialog();

            if (selectEasy.Checked) Difficulty = "easy";
            else if (selectMedium.Checked) Difficulty = "medium";
            else if (selectHard.Checked) Difficulty = "hard";

            void Start(object? sender, EventArgs e)
            {
                intro.Close();
            }
        }

        private void SetScreenSize(int screen, string title, int movesX = 0)
        {
            switch (Difficulty)
            {
                case "easy":
                    Image backgroundImage = Properties.Resources.background_easy;
                    this.BackgroundImage = backgroundImage;
                    break;
                case "medium":
                    backgroundImage = Properties.Resources.background_mid;
                    this.BackgroundImage = backgroundImage;
                    break;
                case "hard":
                    backgroundImage = Properties.Resources.background_hard;
                    this.BackgroundImage = backgroundImage;
                    break;
            }

            //Uses shuffle icon image

            this.ClientSize = new Size(screen, screen);
            this.Text = $"Sliding Puzzle {title} - Fabio Weck";
            lblMoves.Location = new Point(260 + movesX, 35);
        }

        //Set the form inner size, title and background
        private void SetForm(string reset = "no")
        {
            if (reset != "no")
            {
                foreach (Label label in Labels)
                {
                    label.Visible = false;
                }
            }

            switch (Difficulty)
            {
                case "easy":
                    SetScreenSize(600, "4x4", 0);
                    break;

                case "medium":
                    SetScreenSize(700, "5x5", 50);
                    break;

                case "hard":
                    SetScreenSize(800, "6x6", 100);
                    break;
            }

            CenterToScreen();

        }

        private void DefineIcons(Image icon, Button btn, int locationX, int boardAddition, string tooltip)
        {
            Image resizedIcon = new Bitmap(icon, 40, 40);    //Set icon size (width, height)
            btn.Image = resizedIcon;                         //Assign icon to the button
            btn.Location = new Point(locationX, 525 + boardAddition);
            ToolTip shuffleTooltip = new ToolTip();                 //Adds tooltip when te user hovers the pointer over the button
            shuffleTooltip.SetToolTip(btnShuffle, tooltip);
        }

        //Sets the icons for shuffle and sort buttons
        private void SetIconsOnScreen()
        {
            int boardAddition = 0;

            if (Difficulty == "medium")
            {
                boardAddition += 100;
            }
            else if (Difficulty == "hard")
            {
                boardAddition += 200;
            }

            Image shuffleIcon = Properties.Resources.shuffleIcon;   //Uses shuffle icon image
            DefineIcons(shuffleIcon, btnShuffle, 125, boardAddition, "Shuffle all tiles");
            Image sortIcon = Properties.Resources.sortIcon;
            DefineIcons(sortIcon, btnSort, 425 + boardAddition, boardAddition, "Sort all tiles");

        }

        //Method to handle shuffle button
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            ResetMoves();
            ShuffleArray(Labels);                //Shuffles the array
            RenderNumbers();                     //Renders tiles on the screen
        }

        //Method to handle the sort button
        private void btnSort_Click(object sender, EventArgs e)
        {
            ResetMoves();
            DefineArray("reset");
            RenderNumbers();    //Renders the tile on the screen
        }

        //Method to render the numbers on the screen
        private void RenderNumbers(string reset = "no")
        {
            int locationX = 100; //Sets the initial position on the screen
            int locationY = 100;
            int counter = 0;
            int cols = 0;         //Counts the number of iterations in order to
                                  //add vertical space after 4 buttons placed in a row

            if (Difficulty == "easy") cols = 4;
            else if (Difficulty == "medium") cols = 5;
            else if (Difficulty == "hard") cols = 6;

            foreach (Label label in Labels)
            {
                if (counter == cols)
                {
                    locationY += 100; //Adds size to the Y axis
                    locationX = 100;  //Resets X axis to first position
                    counter = 0;      //Resets the counter
                };

                if (label.Name == "lblEmpty")
                {
                    label.BackColor = Color.Transparent; //The only label with transparency
                }

                else
                {
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.BackColor = Color.White;
                }

                label.Size = new Size(100, 100);
                label.Font = new Font("Segoe UI", 25);
                label.ForeColor = Color.Black;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Location = new Point(locationX, locationY);
                label.Visible = true;
                label.BringToFront();

                locationX += 100; //Adds size to the X axis
                counter++;

                if (reset != "no")
                {
                    this.Controls.Add(label);
                }
            }
        }

        private void SetArraySize(int rows, int cols)
        {
            int counter = 0;

            Labels = new Label[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == rows - 1 && j == cols - 1)
                    {
                        Labels[i, j] = AllLabels[35];
                        return;
                    }
                    Labels[i, j] = AllLabels[counter];
                    counter++;
                }
            }
        }

        private void DefineArray(string reset = "no")
        {
            AllLabels = new Label[]
            {
                lblOne, lblTwo, lblThree, lblFour, lblFive, lblSix,
                lblSeven, lblEight, lblNine, lblTen, lblEleven, lblTwelve,
                lblThirteen, lblFourteen, lblFifteen, lblSixteen, lblSeventeen, lblEighteen,
                lblNineteen, lblTwenty, lblTwentyOne, lblTwentyTwo, lblTwentyThree, lblTwentyFour,
                lblTwentyFive, lblTwentySix, lblTwentySeven, lblTwentyEight, lblTwentyNine, lblThirty,
                lblThirtyOne, lblThirtyTwo, lblThirtyThree, lblThirtyFour, lblThirtyFive, lblEmpty
            };

            if (reset == "no")
            {
                foreach (Label label in AllLabels)
                {
                    label.Click += LabelClicked;
                }
            }

            //Defines the array of labels with their names based on difficulty level
            if (Difficulty == "easy")
            {
                SetArraySize(4, 4);
            }

            else if (Difficulty == "medium")
            {
                SetArraySize(5, 5);
            }

            else if (Difficulty == "hard")
            {
                SetArraySize(6, 6);
            }
        }

        /*This method shuffles the labels inside the labels array.
         *That was based on the following source:
         *** http://www.csharphelper.com/howtos/howto_randomize_2d_array.html ***
         */

        public static void ShuffleArray(Label[,] labels)
        {

            int rows = labels.GetUpperBound(0) + 1; //Gets the number os rows and columns
            int cols = labels.GetUpperBound(1) + 1;
            int cells = rows * cols;

            Random rand = new Random(); //Gets random numbers

            for (int i = 0; i < cells - 1; i++)
            {
                int j = rand.Next(i, cells); //Delivers random numbers from 'i' to number os cells
                int row_i = i / cols;        //Defines 2 rows and 2 columns
                int col_i = i % cols;
                int row_j = j / cols;
                int col_j = j % cols;

                Label temp = labels[row_i, col_i];           //Creates a temporary label to store the current label
                labels[row_i, col_i] = labels[row_j, col_j]; //Sets the current label in the array with a randomly defined label
                labels[row_j, col_j] = temp;                 //Changes the random label to the previous stored label
            }
        }

        /*
         * From here, methods were created based on the tile position, covering all the corners, edges and inner positions;
         * The methods were declared clockwise - starts from top-left corner, then top-middle, top-right corner, right-middle...
         * Each case checks the position of the empty tile based on the current/clicked tile. 
         * Having the position defined, the function inside the scope performs a swap between the tile and the empty tile.
         */

        private void LeftUpperCorner(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);

            }
            else if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);

            }
        }

        private void TopMiddle(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);
            }
            else if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);
            }
            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }
        }

        private void TopRightCorner(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }
        }

        private void RightMiddle(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }
        }

        private void BottomRightCorner(int i, int j, Label currentLabel)
        {

            if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }
        }

        private void BottomMiddle(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }
        }

        private void BottomLeftCorner(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }
        }

        private void LeftMiddle(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }
        }

        private void Middle(int i, int j, Label currentLabel)
        {
            if (lblEmpty == Labels[i, j + 1])
            {
                EmptyAtRight(i, j, currentLabel);
            }

            if (lblEmpty == Labels[i - 1, j])
            {
                EmptyAbove(i, j, currentLabel);
            }

            else if (lblEmpty == Labels[i, j - 1])
            {
                EmptyAtLeft(i, j, currentLabel);
            }
            else if (lblEmpty == Labels[i + 1, j])
            {
                EmptyBelow(i, j, currentLabel);
            }
        }

        /*
         Here the methods are based on 4 situations: empty tile can be above, below, at left or at right of the clicked tile.
         Each method will perform its swap between empty and clicked tiles.
         After swapping tiles, the methods render the array again in order to display the change.
         The order of the tiles is also checked to determine if the player managed to finish the game.
         */
        private void EmptyAtRight(int i, int j, Label currentLabel)
        {
            Labels[i, j + 1] = currentLabel;    //Moves the clicked tile to the right
            Labels[i, j] = lblEmpty;            //Moves empty tile to the left
            AddMove();                          //Count one move
            RenderNumbers();                    //Renders the array again
            CheckOrder();                       //Checks win or not
        }

        private void EmptyAtLeft(int i, int j, Label currentLabel)
        {
            Labels[i, j - 1] = currentLabel;    //Moves the clicked tile to the left
            Labels[i, j] = lblEmpty;            //Moves empty tile to the right
            AddMove();
            RenderNumbers();
            CheckOrder();
        }

        private void EmptyBelow(int i, int j, Label currentLabel)
        {
            Labels[i + 1, j] = currentLabel;    //Moves the clicked tile down
            Labels[i, j] = lblEmpty;            //Moves empty tile up
            AddMove();
            RenderNumbers();
            CheckOrder();
        }

        private void EmptyAbove(int i, int j, Label currentLabel)
        {
            Labels[i - 1, j] = currentLabel;    //Moves the clicked tile up
            Labels[i, j] = lblEmpty;            //Moves empty tile down
            Labels[i, j] = lblEmpty;
            AddMove();
            RenderNumbers();
            CheckOrder();
        }

        /*
         * This method handles all clicks on the board casting the sender in order to define which tile was clicked.
         * After finding the tile position in the array, a series of conditional statements also defines the position
         * on the board. Each position has its respective method to move tiles.
         */
        private void LabelClicked(object sender, EventArgs e)
        {
            Label currentLabel = (Label)sender;         //Cast the sender to type of Label

            int rows = Labels.GetUpperBound(0) + 1;     //Gets the number of rows
            int cols = Labels.GetUpperBound(1) + 1;     //Gets the number of columns

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (currentLabel == Labels[i, j])  //finds the position of the clicked tile
                    {
                        if (i == 0 && j == 0)          //Position [0],[0] - First tile on the top-left corner
                        {
                            LeftUpperCorner(i, j, currentLabel);
                            return;
                        }

                        else if (i == 0 && j > 0 && j < cols - 1) //Position [0],[1...2] - Tiles on the middle of the top edge
                        {
                            TopMiddle(i, j, currentLabel);
                            return;
                        }

                        else if (i == 0 && j == cols - 1) //Position [0],[3] - Tile on the top-right corner
                        {
                            TopRightCorner(i, j, currentLabel);
                            return;
                        }

                        else if (i > 0 && i < rows - 1 && j == cols - 1) //Position [1...2],[3] - Tiles on the middle of the right edge
                        {
                            RightMiddle(i, j, currentLabel);
                            return;
                        }

                        else if (i > 0 && i < rows - 1 && j == 0) //Position [1...2],[0] - Tiles on the middle of the left edge
                        {
                            LeftMiddle(i, j, currentLabel);
                            return;
                        }

                        else if (i == rows - 1 && j == 0) //Position [3],[0] - Tile on the bottom-left corner
                        {
                            BottomLeftCorner(i, j, currentLabel);
                            return;
                        }
                        else if (i == rows - 1 && j > 0 && j < cols - 1) //Position [3],[1...2] - Tiles on the middle of the bottom edge
                        {
                            BottomMiddle(i, j, currentLabel);
                            return;
                        }
                        else if (i == rows - 1 && j == cols - 1) //Position [3],[3] - The last tile on the bottom-right corner
                        {
                            BottomRightCorner(i, j, currentLabel);
                            return;
                        }
                        else  //Any tile on the inner middle
                        {
                            Middle(i, j, currentLabel);
                            return;
                        }
                    }
                }
            }
        }

        /*
          This method captures each text of the labels and counts them.
          If it is possible to count 15 times in a row, the player won because tiles are ordered.
          After winning, the player can choose between playing again or exit the game.
        */
        private void CheckOrder()
        {
            int counter = 1;

            int rows = Labels.GetUpperBound(0) + 1; //Gets the number os rows and columns
            int cols = Labels.GetUpperBound(1) + 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (Labels[i, j].Text == counter.ToString())       //Checks labels text
                    {
                        if (counter == Labels.Length - 1 && Labels[rows - 1, cols - 1] == lblEmpty) //Checks if it was possible to count 15 times
                                                                                                    //And if the empty tile is on the last position on the board
                        {
                            DialogResult = MessageBox.Show($"You completed the game after {Moves} moves!\nWould you like to play again?", "Congrats!", MessageBoxButtons.YesNo);

                            if (DialogResult == DialogResult.Yes)   //If the user wants to play again
                            {
                                ResetMoves();                       //Resets the number of moves
                                ShuffleArray(Labels);               //Shuffle the array again
                                RenderNumbers();          //Renders the shuffled array on the screen
                                return;
                            }
                            else //If the user chose to not play again
                            {
                                Application.Exit(); //Closes the window
                            }
                        }

                        counter++; //Increments the counter if 15 was not reached
                    }
                }
            }
        }

        private void AddMove()
        {
            Moves += 1;
            lblMoves.Text = $"Moves: {Moves}";
        }

        private void ResetMoves()
        {
            Moves = 0;
            lblMoves.Text = $"Moves: {Moves}";
        }

        private void ChangeDifficultyAndReset()
        {
            ResetMoves();
            SetForm("reset");
            SetIconsOnScreen();
            DefineArray("reset");
            ShuffleArray(Labels);
            RenderNumbers("reset");
        }

        private void stripMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = (ToolStripMenuItem)sender;

            switch (selectedItem.Text)
            {
                case "Easy":
                    Difficulty = "easy";
                    ChangeDifficultyAndReset();
                    break;

                case "Medium":
                    Difficulty = "medium";
                    ChangeDifficultyAndReset();
                    break;

                case "Hard":
                    Difficulty = "hard";
                    ChangeDifficultyAndReset();
                    break;

                default:
                    Application.Exit();
                    break;
            }
        }
    }
}