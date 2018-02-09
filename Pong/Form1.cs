/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //paddle position variables
        int paddle1X, paddle1Y;
        int paddle2X, paddle2Y;

        //ball position variables
        int ballX, ballY;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = false;

        //constants used to set size and speed of paddles 
        const int PADDLE_LENGTH = 40;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle
        const int PADDLE_SPEED = 4;

        //constants used to set size and speed of ball 
        const int BALL_SIZE = 10;
        int BALL_SPEED = 4;

        //player scores
        int player1Score = 0;
        int player2Score = 0;

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        //game winning score
        int gameWinScore = 2;

        //brush for paint method
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font displayFont = new Font("Arial", 12, FontStyle.Regular);

        Rectangle paddle1Box, paddle2Box, ballBox;

        #endregion

        public Form1()
        {
            InitializeComponent();
            SetParameters();        
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                default:
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets up the game objects in their start position, resets the scores, displays a 
        /// countdown, and then starts the game timer.
        /// </summary>
        private void GameStart()
        {
            newGameOk = true;
            SetParameters();

            startLabel.Visible = false;

            gameUpdateLoop.Start();
            newGameOk = false;

            this.BackColor = Color.Blue;
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                //set score variables to 0
                player1Score = player2Score = 0;

                // set both paddle y positions to middle of screen
                paddle1Y = paddle2Y = this.Height / 2 - PADDLE_LENGTH / 2;

                // set x locations for both paddles
                paddle1X = PADDLE_EDGE;
                paddle2X = this.Width - PADDLE_EDGE - PADDLE_WIDTH;
            }
            BALL_SPEED = 4;
            ballX = this.Width / 2; ballY = this.Height / 2;        
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            //sound player to be used for all in game sounds initially set to collision sound
            SoundPlayer player, paddlehit, score = new SoundPlayer();
            player = new SoundPlayer(Properties.Resources.collision);
            paddlehit = new SoundPlayer(Properties.Resources.collision);    //can't seem to reference "wall bounce.wav" "or paddlehit.wav"
            score = new SoundPlayer(Properties.Resources.score);

            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and BALL_SPEED
            if (ballMoveRight) { ballX += BALL_SPEED; }
            else if (!ballMoveRight) { ballX -= BALL_SPEED; }
            // TODO create code move ball either down or up based on ballMoveDown and BALL_SPEED
            if (ballMoveDown) { ballY += BALL_SPEED + 1 ; }
            else if (!ballMoveDown) { ballY -= BALL_SPEED + 1; }

            #endregion

            #region update paddle positions
           
            if (aKeyDown == true && paddle1Y > 0)   //paddle 1 movement logic
            {
                paddle1Y -= PADDLE_SPEED;
            }
            if (zKeyDown == true && paddle1Y + PADDLE_LENGTH < this.Height)
            {
                paddle1Y += PADDLE_SPEED;
            }

            if (jKeyDown == true && paddle2Y > 0)   //paddle 2 movement logic
            {
                paddle2Y -= PADDLE_SPEED;
            }
            if (mKeyDown == true && paddle2Y + PADDLE_LENGTH < this.Height)
            {
                paddle2Y += PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ballY < 0) // if ball hits top line
            {
                ballMoveDown = true;
                player.Play();
            }
            else if (ballY + BALL_SIZE > this.Height) // if ball hits bottom line
            {
                ballMoveDown = false;
                player.Play();
            }

            #endregion

            #region ball collision with paddles

            ballBox = new Rectangle(ballX, ballY, BALL_SIZE, BALL_SIZE);
            paddle1Box = new Rectangle(paddle1X, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH);
            paddle2Box = new Rectangle(paddle2X, paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH);

            if (ballBox.IntersectsWith(paddle1Box)) //left paddle contact
            {
                ballMoveRight = true;
                paddlehit.Play();
            }

            if (ballBox.IntersectsWith(paddle2Box)) //right paddle contact
            {
                ballMoveRight = false;
                paddlehit.Play();
            }          

            #endregion

            #region ball collision with side walls (point scored)

            if (ballX < 0)  // TODO ball hits left wall logic
            {
                score.Play();
                player2Score++;

                if (player2Score >= gameWinScore)
                {
                    GameOver("Player 2 Win!");
                    score.Stop();
                }
                else if (player2Score < gameWinScore) { ballMoveRight = true; BALL_SPEED++; SetParameters(); }
            }

            if (ballX + BALL_SIZE > this.Width)  // TODO ball hits right wall logic
            {
                score.Play();
                player1Score++;

                if (player1Score >= gameWinScore)
                {
                    GameOver("Player 1 Win!");
                    score.Stop();
                }
                else if (player1Score < gameWinScore) { ballMoveRight = false; BALL_SPEED++; SetParameters(); }
            }

            // TODO same as above but this time check for collision with the right wall

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;

            gameUpdateLoop.Stop();
            startLabel.Visible = true;
            startLabel.Text = winner;

            this.Refresh();
            Thread.Sleep(2000);            
            startLabel.Text = "Play again? Y/N";
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw paddles using FillRectangle
            e.Graphics.FillRectangle(drawBrush, paddle1X, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH);
            e.Graphics.FillRectangle(drawBrush, paddle2X, paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH);
            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(drawBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
            // TODO draw scores to the screen using DrawString
            e.Graphics.DrawString("Player 1 Score: " + Convert.ToString(player1Score), displayFont, drawBrush, 35, 10);
            e.Graphics.DrawString("Player 2 Score: " + Convert.ToString(player2Score), displayFont, drawBrush, this.Width - 175, 10);
        }

    }
}
