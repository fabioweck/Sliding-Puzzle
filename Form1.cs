using System.Diagnostics.Metrics;
using System.Reflection;

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
            SetIcons();
            DefineArray();
            ShuffleArray(Labels);
            RenderNumbers();
        }

        private void Intro()
        {
            intro = new();
            intro.Size = new Size(300, 300);
            intro.StartPosition = FormStartPosition.CenterScreen;

            Label selectDifficulty = new();
            selectDifficulty.AutoSize = true;
            selectDifficulty.Font = new Font("Tw Cen MT Condensed Extra Bold", 12);
            selectDifficulty.Location = new Point(130 - selectDifficulty.Width / 2, 15);
            selectDifficulty.Text = "Select difficulty";

            RadioButton selectEasy = new();
            selectEasy.Location = new Point(100, 60);
            selectEasy.Text = "Easy";
            selectEasy.Checked = true;

            RadioButton selectMedium = new();
            selectMedium.Text = "Medium";
            selectMedium.Location = new Point(100, 100);

            RadioButton selectHard = new();
            selectHard.Text = "Hard";
            selectHard.Location = new Point(100, 140);


            Button start = new();
            start.Size = new Size(50, 30);
            start.Location = new Point(110, 200);
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
        }

        private void Start(object? sender, EventArgs e)
        {
            intro.Close();
        }

        //Set the form inner size, title and background
        private void SetForm()
        {

            Image backgroundImage = Properties.Resources.background;   //Uses shuffle icon image
            Image resizedImage = new Bitmap(backgroundImage, 406, 406);

            if (Difficulty == "easy")
            {
                this.ClientSize = new Size(600, 600);
                this.BackgroundImage = resizedImage;
                this.Text = "Sliding Puzzle 4x4 - Fabio Weck";
                lblMoves.Location = new Point(257, 18);
                CenterToScreen();
            }

            else if (Difficulty == "medium")
            {
                this.ClientSize = new Size(700, 700);
                resizedImage = new Bitmap(backgroundImage, 506, 506);
                this.BackgroundImage = resizedImage;
                this.Text = "Sliding Puzzle 5x5 - Fabio Weck";
                lblMoves.Location = new Point(307, 18);
                CenterToScreen();
            }

            else if (Difficulty == "hard")
            {
                this.ClientSize = new Size(800, 800);
                resizedImage = new Bitmap(backgroundImage, 606, 606);
                this.BackgroundImage = resizedImage;
                this.Text = "Sliding Puzzle 6x6 - Fabio Weck";
                lblMoves.Location = new Point(357, 18);
                CenterToScreen();
            }

        }

        //Sets the icons for shuffle and sort buttons
        private void SetIcons()
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
            Image resizedIcon = new Bitmap(shuffleIcon, 40, 40);    //Set icon size (width, height)
            btnShuffle.Image = resizedIcon;                         //Assign icon to the button
            btnShuffle.Location = new Point(125, 525 + boardAddition);
            ToolTip shuffleTooltip = new ToolTip();                 //Adds tooltip when te user hovers the pointer over the button
            shuffleTooltip.SetToolTip(btnShuffle, "Shuffle all tiles");

            Image sortIcon = Properties.Resources.sortIcon;
            resizedIcon = new Bitmap(sortIcon, 40, 40);
            btnSort.Image = resizedIcon;
            btnSort.Location = new Point(425 + boardAddition, 525 + boardAddition);
            ToolTip sortTooltip = new ToolTip();
            sortTooltip.SetToolTip(btnSort, "Sort all tiles");
        }

        //Method to handle shuffle button
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            Moves = 0;                           //Resets number of moves
            lblMoves.Text = $"Moves: {Moves}";
            ShuffleArray(Labels);                //Shuffles the array
            RenderNumbers();                     //Renders tiles on the screen
        }

        //Method to handle the sort button
        private void btnSort_Click(object sender, EventArgs e)
        {
            Moves = 0;                           //Resets number of moves
            lblMoves.Text = $"Moves: {Moves}";
            DefineArray("reset");
            RenderNumbers();    //Renders the tile on the screen
        }

        //Method to render the numbers on the screen
        private void RenderNumbers()
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
                label.BringToFront();

                locationX += 100; //Adds size to the X axis
                counter++;
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

            //Defines the array of labels with their names 
            if (Difficulty == "easy")
            {
                int counter = 0;

                Labels = new Label[4, 4];

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (i == 3 && j == 3)
                        {
                            Labels[i, j] = AllLabels[35];

                            //Assigns each label to the LabelClick method
                            if (reset == "no")
                            {
                                foreach (Label label in Labels)
                                {
                                    label.Click += LabelClicked;
                                }

                                return;
                            }
                            else    //If it is a reset, just returns without reassigning the method
                            {
                                return;
                            }

                        }
                        Labels[i, j] = AllLabels[counter];
                        counter++;
                    }
                }
            }

            else if (Difficulty == "medium")
            {
                int counter = 0;

                Labels = new Label[5, 5];

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 4 && j == 4)
                        {
                            Labels[i, j] = AllLabels[35];

                            //Assigns each label to the LabelClick method
                            if (reset == "no")
                            {
                                foreach (Label label in Labels)
                                {
                                    label.Click += LabelClicked;
                                }
                                return;
                            }
                            else    //If it is a reset, just returns without reassigning the method
                            {
                                return;
                            }
                        }
                        Labels[i, j] = AllLabels[counter];
                        counter++;
                    }
                }
            }

            else if (Difficulty == "hard")
            {
                int counter = 0;

                Labels = new Label[6, 6];

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Labels[i, j] = AllLabels[counter];
                        counter++;
                    }
                }

                if (reset == "no")
                {
                    foreach (Label label in Labels)
                    {
                        label.Click += LabelClicked;
                    }

                    return;
                }
                else
                {
                    return;
                }
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
            Moves += 1;                         //Count one move
            lblMoves.Text = $"Moves: {Moves}";
            RenderNumbers();                    //Renders the array again
            CheckOrder();                       //Checks win or not
        }

        private void EmptyAtLeft(int i, int j, Label currentLabel)
        {
            Labels[i, j - 1] = currentLabel;    //Moves the clicked tile to the left
            Labels[i, j] = lblEmpty;            //Moves empty tile to the right
            Moves += 1;
            lblMoves.Text = $"Moves: {Moves}";
            RenderNumbers();
            CheckOrder();
        }

        private void EmptyBelow(int i, int j, Label currentLabel)
        {
            Labels[i + 1, j] = currentLabel;    //Moves the clicked tile down
            Labels[i, j] = lblEmpty;            //Moves empty tile up
            Moves += 1;
            lblMoves.Text = $"Moves: {Moves}";
            RenderNumbers();
            CheckOrder();
        }

        private void EmptyAbove(int i, int j, Label currentLabel)
        {
            Labels[i - 1, j] = currentLabel;    //Moves the clicked tile up
            Labels[i, j] = lblEmpty;            //Moves empty tile down
            Labels[i, j] = lblEmpty;
            Moves += 1;
            lblMoves.Text = $"Moves: {Moves}";
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
                                Moves = 0;                          //Resets the number of moves
                                lblMoves.Text = $"Moves: {Moves}";
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
    }
}