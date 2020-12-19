using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace HangMan
{
    public partial class Form1 : Form
    {
        string word; //use this for the random word generated
        int amount = 0; //use this for the amount of wrong attempts
        bool isOver = false;  //use this to enter game over mode
        List<Label> listLabels = new List<Label>();
        public Form1()
        {
            InitializeComponent();
        }

        //this is used to identify the main parts of the hangman
        enum BodyParts
        {
            Head,
            Left_Eye,
            Right_Eye,
            Mouth,
            Right_Arm,
            Left_Arm,
            Body,
            Right_Leg,
            Left_Leg
        }

        //this method is to implement the hangpost drawing
        void drawHangPost()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Black, 10);

            g.DrawLine(p, new Point(130, 351), new Point(130, 5));   //original size is 149, 218
            g.DrawLine(p, new Point(135, 5), new Point(65, 5));
            g.DrawLine(p, new Point(60, 0), new Point(60, 50));

        }

        //here draw the body parts specified in the BodyParts enum
        void drawBodyPart(BodyParts bp)
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Blue, 2);

            if(bp == BodyParts.Head)
            {
                g.DrawEllipse(p, 40, 50, 40, 40);
            }

            else if (bp == BodyParts.Left_Eye)
            {
                SolidBrush s = new SolidBrush(Color.Blue);
                g.FillEllipse(s, 50, 60, 5, 5);
            }

            else if(bp == BodyParts.Right_Eye)
            {
                SolidBrush s = new SolidBrush(Color.Blue);
                g.FillEllipse(s, 64, 60, 5, 5);
            }

            else if (bp == BodyParts.Mouth)
            {
                g.DrawArc(p, 50, 60, 20, 20, 45, 90);
            }

            else if(bp == BodyParts.Body)
            {
                g.DrawLine(p, new Point(60, 90), new Point(60, 170));
            }

            else if(bp == BodyParts.Left_Arm)
            {
                g.DrawLine(p, new Point(60, 110), new Point(30, 95));
            }

            else if(bp == BodyParts.Right_Arm)
            {
                g.DrawLine(p, new Point(60, 110), new Point(90, 95));
            }

            else if(bp == BodyParts.Left_Leg)
            {
                g.DrawLine(p, new Point(60, 170), new Point(30, 190));
            }

            else if(bp == BodyParts.Right_Leg)
            {
                g.DrawLine(p, new Point(60, 170), new Point(90, 190));
            }
        }

        //this will handle the labels
        void makeLabels()
        {
            word = getRandomWord();
            char[] chars = word.ToCharArray();
            int space = 325 / chars.Length;

            for(int i = 0; i < chars.Length; i++)
            {
                listLabels.Add(new Label());
                listLabels[i].Location = new Point((i * space) + 5, 40);
                listLabels[i].Text = "_";
                listLabels[i].Parent = groupBox2;
                listLabels[i].BringToFront();
                listLabels[i].CreateControl();
            }

            lblength.Text = "Word Length: " + chars.Length;
        }

        //get a list of random words and return a random word from that list
        string getRandomWord()
        {
            WebClient wc = new WebClient();
            string wordList = wc.DownloadString("https://www-cs-faculty.stanford.edu/~knuth/sgb-words.txt");
            string[] words = wordList.Split('\n');
            Random rand = new Random();
            return words[rand.Next(0, words.Length - 1)];
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            drawHangPost();
            makeLabels();
        }

        private void btnLetter_Click(object sender, EventArgs e)
        {
            if (txtLetter.Text.Length > 0 && isOver == false)
            {
                char letter = txtLetter.Text.ToLower().ToCharArray()[0];
                if (!char.IsLetter(letter))
                {
                    MessageBox.Show("Only letters allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (word.Contains(letter))
                {
                    char[] letters = word.ToCharArray();
                    for(int i = 0; i < letters.Length; i++)
                    {
                        if(letters[i] == letter)
                        {
                            listLabels[i].Text = letter.ToString();
                        }
                    }
                    //looping through the labels, if no labels have _ then user has won
                    for(int i = 0; i < listLabels.Capacity; i++)
                    {
                        if(listLabels[i].Text == "_")
                        {
                            return;
                        }

                        MessageBox.Show("You Won!", "Congratulations");
                        isOver = true;
                       

                    }
                }

                else  //user is allowed to guess up to 9 times incorrectly, once the hangman is fully drawn, game ends
                {
                    MessageBox.Show("Wrong Guess", "Attempt");
                    lblMissed.Text += "" + letter.ToString() + ", ";
                    drawBodyPart((BodyParts)amount);
                    amount++;
                    if(amount == 9)
                    {
                        MessageBox.Show("Game Over \nWord: " + word);
                        isOver = true;
                       
                    }
                }
            }
        }

        //resetting
        void resetGame()
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(panel1.BackColor);
            getRandomWord();
            makeLabels();
            drawHangPost();
            lblMissed.Text = "Missed: ";
            txtLetter.Text = "";
            amount = 0;
            isOver = false;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            resetGame();
        }

        private void btnWord_Click(object sender, EventArgs e)
        {
            if(txtWord.Text.Length > 0 && isOver == false)
            {
                for(int i = 0; i < txtWord.Text.Length; i++)
                {
                    if (!char.IsLetter(txtWord.Text[i]))
                    {
                        MessageBox.Show("Only letters allowed");
                        return;
                    }
                }
                if(txtWord.Text.ToLower() == word)
                {
                    MessageBox.Show("You won!", "Congratulations");
                    isOver = true;
                }

                else
                {
                    MessageBox.Show("Wrong guess", "Attempt");
                    drawBodyPart((BodyParts)amount);
                    amount++;
                    
                    if(amount == 9)
                    {
                        MessageBox.Show("Game Over \nWord: " + word);
                        isOver = true;
                    }
                }
            }
        }

       
                
            
        
    }
}
