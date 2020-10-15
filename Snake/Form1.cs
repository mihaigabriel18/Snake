using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{

    public partial class Snake : Form
    {
        // enable double buffering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        static Color[,] mapColor = new Color[30, 30];
        static Queue position = new Queue();
        public static bool tick = false;
        public static int direction; // 0 for up // 1 for right // 2 for down // 3 for left
        public static int score = 0;
        public static bool isFirst = true;

        struct coords {
            public int x, y;
        };

        static coords justEnqueuedItem; // the item that was just enqueued (a peek that works on the other side)

        public Snake()
        {
            // initializing the starting position of the snake
            initilizeSnake(mapColor);
            InitializeComponent();

            timer1.Interval = 100;
            timer1.Tick += Timer1_Tick;
        }

        private void initilizeSnake(Color[,] color)
        {

            // we put the snake in a random position !!!!!!!!
            coords point = new coords();
            justEnqueuedItem = new coords();
            direction = 1;

            point.x = 2;
            point.y = 14;
            position.Enqueue(point);
            color[point.x, point.y] = Color.Green;

            point.x++;
            position.Enqueue(point);
            color[point.x, point.y] = Color.Green;

            point.x++;
            position.Enqueue(point);
            color[point.x, point.y] = Color.Green;

            point.x++;
            position.Enqueue(point);
            justEnqueuedItem = point;
            color[point.x, point.y] = Color.Green;

            generateFood();
        }

        

        private void TableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {

            using (var b = new SolidBrush(mapColor[e.Column, e.Row]))
            {
                e.Graphics.FillRectangle(b, e.CellBounds);
            }
        }

        private bool AmIStillAlive(coords nextPos)
        {

            if (nextPos.x < 0 || nextPos.x >= 30 || nextPos.y < 0 || nextPos.y >= 30)
                return false;

            if (mapColor[nextPos.x, nextPos.y] == Color.Green)
                return false;

            return true;
        }

        //Load list of available Keyboard buttons
        private static Hashtable keyTable = new Hashtable();

        //Perform a check to see if a particular button is pressed.
        public static bool KeyPressed(Keys key)
        {
            if (keyTable[key] == null)
            {
                return false;
            }

            if ((bool)keyTable[key] == true)
            {
                keyTable[key] = false;
                return true;
            }

            return false;
        }

        private void ChangeState(Keys key, bool state)
        {

            keyTable[key] = state;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isFirst)
            {
                if (e.KeyCode.Equals(Keys.Down) || e.KeyCode.Equals(Keys.Up) || e.KeyCode.Equals(Keys.Left) || e.KeyCode.Equals(Keys.Right))
                {
                    timer1.Start();
                    isFirst = false;
                }
            }
            else
            {
                ChangeState(e.KeyCode, true);
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //ChangeState(e.KeyCode, true);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            coords newItem = new coords();

            if (KeyPressed(Keys.Right) && direction != 3)
                direction = 1;
            else if (KeyPressed(Keys.Left) && direction != 1)
                direction = 3;
            else if (KeyPressed(Keys.Up) && direction != 2)
                direction = 0;
            else if (KeyPressed(Keys.Down) && direction != 0)
                direction = 2;

            switch (direction)
            {
                case 0:
                    newItem.y = justEnqueuedItem.y - 1;
                    newItem.x = justEnqueuedItem.x;
                    break;
                case 1:
                    newItem.x = justEnqueuedItem.x + 1;
                    newItem.y = justEnqueuedItem.y;
                    break;
                case 2:
                    newItem.x = justEnqueuedItem.x;
                    newItem.y = justEnqueuedItem.y + 1;
                    break;
                case 3:
                    newItem.x = justEnqueuedItem.x - 1;
                    newItem.y = justEnqueuedItem.y;
                    break;
            }
            // we check if we just killed ourself
            if (!AmIStillAlive(newItem))
            {
                richTextBox5.Visible = true;
                richTextBox4.Visible = false;
                timer1.Stop();
                return;
            }

            //we do those either way


            if (mapColor[newItem.x, newItem.y] == Color.Red)
            {
                justEnqueuedItem = newItem;
                mapColor[justEnqueuedItem.x, justEnqueuedItem.y] = Color.Green;
                position.Enqueue(newItem);
                score++;
                richTextBox2.Text = score.ToString();

                generateFood();
                return;
            }

            justEnqueuedItem = newItem;
            position.Enqueue(newItem);
            mapColor[justEnqueuedItem.x, justEnqueuedItem.y] = Color.Green;
            newItem = (coords)position.Peek();
            mapColor[newItem.x, newItem.y] = Color.White;
            position.Dequeue();
            keyTable.Clear();

            tableLayoutPanel1.Refresh();
        }

        private void generateFood()
        {
            Random randomX = new Random();
            int randomNumberX = randomX.Next(0, 30);
            Random randomY = new Random();
            int randomNumberY = randomY.Next(0, 30);

            while (mapColor[randomNumberX, randomNumberY] == Color.Green)
            {
                randomX = new Random();
                randomNumberX = randomX.Next(0, 30);
                randomY = new Random();
                randomNumberY = randomY.Next(0, 30);
            }

            mapColor[randomNumberX, randomNumberY] = Color.Red;
        }

    }
}
