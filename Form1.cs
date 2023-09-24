using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;

/*
    SODV2101 23SEPMNRT5 : Rapid Application Development
    Fabio Augusto Weck
    #441977

    *** References : http://www.csharphelper.com/howtos/howto_randomize_2d_array.html ***

 */

namespace Sliding_puzzle
{
    public partial class Form1 : Form
    {
        public Label[,] Labels { get; set; }            //Set of labels (tiles) used in the game
        public Label[] AllLabels { get; set; }          //Stores all labels available in the game (36 tiles)
        public int Moves { get; set; } = 0;             //Counts the moves when a player click on a tile    
        public string Difficulty { get; set; }          //Global variable which dictates many methods based on game level
        public Form Intro { get; set; }                 //Intro pop-up

        public Form1()
        {

            InitializeComponent();

            //Set of methods and instructions to initialize the board

            IntroPopUp();               //Intro form (pop-up)
            SetForm();                  //Sets the main form
            SetBtnOnScreen();           //Sets the buttons on main screen
            DefineArray();              //Defines the array size and its items
            ShuffleArray(Labels);       //Shuffles the array
            RenderNumbers();            //Method to correctly display tiles on screen
        }


        //This method defines the pop-up screen upon game load
        private void IntroPopUp()
        {
            Intro = new();                       //Defines the window size and details
            Intro.Size = new Size(350, 300);
            Intro.Text = "Sliding puzzle!";
            Intro.Icon = Properties.Resources.puzzle_icon;
            Intro.StartPosition = FormStartPosition.CenterScreen;

            Label selectDifficulty = new();      //Title
            selectDifficulty.AutoSize = true;
            selectDifficulty.Font = new Font("Tw Cen MT Condensed Extra Bold", 12);
            selectDifficulty.Location = new Point(150 - (selectDifficulty.Width / 2), 15);
            selectDifficulty.Text = "Select difficulty";

            RadioButton selectEasy = new();     //Radio button for "easy" game
            selectEasy.Location = new Point(125, 60);
            selectEasy.Text = "Easy";
            selectEasy.Checked = true;          //Starts checked

            RadioButton selectMedium = new();   //Radio button for "medium" game
            selectMedium.Text = "Medium";
            selectMedium.Location = new Point(125, 100);

            RadioButton selectHard = new();     //Radio button for "hard" game
            selectHard.Text = "Hard";
            selectHard.Location = new Point(125, 140);

            Button start = new();               //Start game button
            start.Size = new Size(50, 30);
            start.Location = new Point(130, 200);
            start.Text = "Start";
            start.Click += Start;

            Intro.Controls.Add(selectDifficulty);   //Adds all elements to the form
            Intro.Controls.Add(selectEasy);
            Intro.Controls.Add(selectMedium);
            Intro.Controls.Add(selectHard);
            Intro.Controls.Add(start);

            CenterToScreen();                       //Initializes the screen on center
            Intro.ShowDialog();

            if (selectEasy.Checked) Difficulty = "easy";            //Checks which level has been selected
            else if (selectMedium.Checked) Difficulty = "medium";
            else if (selectHard.Checked) Difficulty = "hard";

            void Start(object? sender, EventArgs e)                 //Unique function assigned to start button to close the pop-up screen
            {
                Intro.Close();
            }
        }

        /*
         Method to define the screen size based on difficulty
         Each level requires a screen size and a specific items position (x and y) related to its size
         as well as the background image.
        */
        private void SetScreenSize(int screen, string title, int movesX = 0)
        {
            switch (Difficulty)         //Selects the correct background image for the screen size
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

            this.ClientSize = new Size(screen, screen);         //'ClientSize' defines the content/inner size and does not include the title
            this.Text = $"Sliding Puzzle {title} - Fabio Weck"; //Changes the title (4x4, 5x5 or 6x6 boards)
            lblMoves.Location = new Point(260 + movesX, 35);    //Location starts at 260 (X axis) and increments with larger screens
        }

        //Set the form inner size, title and background
        private void SetForm(string reset = "no")
        {

            if (reset != "no")                      //This conditional checks if the game is a reset. Once the tiles are loaded and displayed on screen,
            {                                       //a bug occurs everytime the player changes form hard to mid or easy levels. The tiles remain on screen                                                   
                foreach (Label label in Labels)     //and must be hidden again to avoid this problem.
                {
                    label.Visible = false;
                }
            }

            switch (Difficulty)                     //Sets the size for each level, their titles and elements increments
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

            CenterToScreen();       //Centers the form on the screen
        }

        //Method to load images and define buttons on screen
        private void DefineBtnIcons(Image icon, Button btn, int locationX, int boardAddition, string tooltip)
        {
            Image resizedIcon = new Bitmap(icon, 40, 40);               //Set icon size (width, height)
            btn.Image = resizedIcon;                                    //Assign icon to the button
            btn.Location = new Point(locationX, 525 + boardAddition);
            ToolTip shuffleTooltip = new ToolTip();                     //Adds tooltip when the user hovers the pointer over the button
            shuffleTooltip.SetToolTip(btn, tooltip);
        }

        //Sets the icons for shuffle and sort buttons
        private void SetBtnOnScreen()
        {
            int boardAddition = 0;              //Variable used to add position increments to screen size

            if (Difficulty == "medium")         //Increments based on level
            {
                boardAddition += 100;
            }
            else if (Difficulty == "hard")
            {
                boardAddition += 200;
            }

            Image shuffleIcon = Properties.Resources.shuffleIcon;                               //Shuffle icon image
            DefineBtnIcons(shuffleIcon, btnShuffle, 125, boardAddition, "Shuffle all tiles");
            Image sortIcon = Properties.Resources.sortIcon;                                     //Sort icon image
            DefineBtnIcons(sortIcon, btnSort, 425 + boardAddition, boardAddition, "Sort all tiles");

        }

        //Method to handle shuffle button
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            ResetMoves();                        //Resets the number of moves
            ShuffleArray(Labels);                //Shuffles the array
            RenderNumbers();                     //Renders tiles on the screen
        }

        //Method to handle the sort button
        private void btnSort_Click(object sender, EventArgs e)
        {
            ResetMoves();
            DefineArray("reset");
            RenderNumbers();
        }

        //Method to render the numbers on the screen
        private void RenderNumbers()
        {
            int locationX = 100;    //Sets the initial position on the screen
            int locationY = 100;
            int counter = 0;
            int cols = 0;           //Counts the number of iterations in order to
                                    //add vertical space after 4, 5 or 6 tiles placed in a row

            if (Difficulty == "easy") cols = 4;             //Each level sets a number of tiles in a row
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
                    label.BackColor = Color.Transparent;            //The only label with transparency
                }

                else
                {
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.BackColor = Color.Gainsboro;
                }

                label.Size = new Size(100, 100);
                label.Font = new Font("Segoe UI", 25);
                label.ForeColor = Color.Black;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Location = new Point(locationX, locationY);
                label.Visible = true;                               //This property is important to be set true, otherwise none of the tiles would be displayed
                label.BringToFront();                               //This method guarantees that the tile will be displayed over any other elements

                locationX += 100;                                   //Adds size to the X axis
                counter++;                                          //Counter used to skip rows
            }
        }

        //This method was written to avoid code redundancy inside DefineArray() method
        //It defines the main array size and its elements based on rows and cols arguments
        private void SetArraySize(int rows, int cols)
        {
            int counter = 0;                                //Support variable used to iterates over 'AllLabels' array. That's necessary due to the fact that
                                                            //this array is a conventional array and the main 'Labels' array is multi-dimensional, so
                                                            //the nest loop doesn't offer any suitable number during the loop to go through 'AllLabels'
            Labels = new Label[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == rows - 1 && j == cols - 1)     //If that's the last index of 'Labels', gets the last element of 'AllLabels'.
                    {                                       //The last element must be 'lblEmpty' and it is located in the index 35.
                        Labels[i, j] = AllLabels[35];
                        return;
                    }
                    Labels[i, j] = AllLabels[counter];
                    counter++;
                }
            }
        }

        //Defines the array to be used by the game
        private void DefineArray(string reset = "no")
        {
            AllLabels = new Label[]
            {
                lblOne, lblTwo, lblThree, lblFour, lblFive, lblSix,
                lblSeven, lblEight, lblNine, lblTen, lblEleven, lblTwelve,
                lblThirteen, lblFourteen, lblFifteen, lblSixteen, lblSeventeen, lblEighteen,
                lblNineteen, lblTwenty, lblTwentyOne, lblTwentyTwo, lblTwentyThree, lblTwentyFour,
                lblTwentyFive, lblTwentySix, lblTwentySeven, lblTwentyEight, lblTwentyNine, lblThirty,
                lblThirtyOne, lblThirtyTwo, lblThirtyThree, lblThirtyFour, lblThirtyFive, /* Last element */ lblEmpty /* Empty tile */
            };

            if (reset == "no")                          //If it is the first game, this conditional sets all elements to 'not visible' and assigns
            {                                           //each one the to the main method. If it is not a reset, the code skips this part to avoid bugs.
                foreach (Label label in AllLabels)
                {
                    label.Click += LabelClicked;
                    label.Visible = false;
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
         * 
         *      ------------------- That was based on a web source ---------------------
         *          References:
                --- http://www.csharphelper.com/howtos/howto_randomize_2d_array.html ---
                ------------------------------------------------------------------------
         *
         */
        private void ShuffleArray(Label[,] labels)
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

        //Method to add a move to the moves counter and avoid redundancy through the code
        private void AddMove()
        {
            Moves += 1;
            lblMoves.Text = $"Moves: {Moves}";
        }

        //Method to reset moves counter and avoid redundancy through the code
        private void ResetMoves()
        {
            Moves = 0;
            lblMoves.Text = $"Moves: {Moves}";
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
                            return;                                 //All returns are necessary to stop iteration and avoid bugs/crashes
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
          If it is possible to count determined times in a row (array size defined), tiles are ordered correctly.
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
                        {                                                                           //and if the empty tile is on the last position on the board
                            PlayAgain();
                            return;
                        }

                        counter++; //Increments the counter if 15 was not reached
                    }
                }
            }
        }

        private void PlayAgain()
        {
            DialogResult = MessageBox.Show($"You completed the game after {Moves} moves!\n"
                                          + "Would you like to play again?",
                                          "Congratulations!",
                                          MessageBoxButtons.YesNo);

            if (DialogResult == DialogResult.Yes)   //If the user wants to play again
            {
                ResetMoves();                       //Resets the number of moves
                ShuffleArray(Labels);               //Shuffle the array again
                RenderNumbers();                    //Renders the shuffled array on the screen
                return;
            }
            else //If the user chose not playing again
            {
                Application.Exit();                 //Closes the window
            }
        }

        //Method to call many others methods necessary to perform a reset and avoid code redundancy
        private void ChangeDifficultyAndReset()
        {
            ResetMoves();               //Here the 'reset' string is necessary to sinalize the methods to perform tasks differently
            SetForm("reset");
            SetBtnOnScreen();
            DefineArray("reset");
            ShuffleArray(Labels);
            RenderNumbers();
        }

        //This method handles all elements of strip menu
        private void stripMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = (ToolStripMenuItem)sender;     //Casts the sender to strip menu item
            switch (selectedItem.Text)                                      //Checks which item was selected and sets the game accordingly
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

                default:                                                    //If the item selected is not any of the level selection, then it is the 'exit'
                    Application.Exit();                                     //Closes the application
                    break;
            }
        }

        //Courtesy window to display project info
        private void AboutPopUp(object sender, EventArgs e)
        {
            MessageBox.Show("SODV2101: Rapid Application Development\n"
                            + "23SEPMNRT5\n"
                            + "Assignment 01 : Sliding Puzzle game\n"
                            + "#441977",
                            "Fabio Augusto Weck",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}