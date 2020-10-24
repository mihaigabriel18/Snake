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
        // Checking if the difficulty has been selected
        private bool difficultySelected;
        // The score of the current game
        private static int score;
        // We check if we are at the first buttom press, the one that start the game
        private static bool startGame;
        // Instance of the snake
        private SnakeInstance snake;

        // Default constructor
        public Snake()
        {
            difficultySelected = false;
            snake = new SnakeInstance("RIGHT");

            score = 0;
            startGame = true;

            // initializing the starting position of the snake
            InitializeComponent();

            timer1.Tick += Timer1_Tick;
        }

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

        //static coords justEnqueuedItem; // the item that was just enqueued (a peek that works on the other side)

        

        private void TableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {

            using (var b = new SolidBrush(snake.MapColor[e.Column, e.Row]))
            {
                e.Graphics.FillRectangle(b, e.CellBounds);
            }
        }

        private bool AmIStillAlive(Tuple<int, int> nextPos)
        {

            if (nextPos.Item1 < 0 || nextPos.Item1 >= SnakeInstance.GridSize ||
                nextPos.Item2 < 0 || nextPos.Item2 >= SnakeInstance.GridSize)
                return false;

            if (snake.MapColor[nextPos.Item1, nextPos.Item2] == Color.Green)
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
            if (difficultySelected)
            {
                if (startGame)
                {
                    if (e.KeyCode.Equals(Keys.Down) || e.KeyCode.Equals(Keys.Up) ||
                        e.KeyCode.Equals(Keys.Left) || e.KeyCode.Equals(Keys.Right))
                    {
                        timer1.Start();
                        startGame = false;
                    }
                }
                else
                {
                    ChangeState(e.KeyCode, true);
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //ChangeState(e.KeyCode, true);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Tuple <int, int> newItem;

            if (KeyPressed(Keys.Right) && snake.Direction != 3)
                snake.Direction = 1;
            else if (KeyPressed(Keys.Left) && snake.Direction != 1)
                snake.Direction = 3;
            else if (KeyPressed(Keys.Up) && snake.Direction != 2)
                snake.Direction = 0;
            else if (KeyPressed(Keys.Down) && snake.Direction != 0)
                snake.Direction = 2;

            switch (snake.Direction)
            {
                case 0:
                    newItem = new Tuple<int, int>(snake.JustEnqueuedItem.Item1, snake.JustEnqueuedItem.Item2 - 1);
                    break;
                case 1:
                    newItem = new Tuple<int, int>(snake.JustEnqueuedItem.Item1 + 1, snake.JustEnqueuedItem.Item2);
                    break;
                case 2:
                    newItem = new Tuple<int, int>(snake.JustEnqueuedItem.Item1, snake.JustEnqueuedItem.Item2 + 1);
                    break;
                case 3:
                    newItem = new Tuple<int, int>(snake.JustEnqueuedItem.Item1 - 1, snake.JustEnqueuedItem.Item2);
                    break;
                default: // never going to reach here but we make an exception for this
                    newItem = new Tuple<int, int>(0, 0);
                    throw new ArgumentException("Invalid direction: ", snake.Direction.ToString());
            }

            // we check if we just killed ourself
            if (!AmIStillAlive(newItem))
            {
                richTextBox5.Visible = true;
                richTextBox4.Visible = false;
                button1.Visible = true;
                timer1.Stop();
                return;
            }

            //we do those either way


            if (snake.MapColor[newItem.Item1, newItem.Item2] == Color.Red)
            {
                snake.JustEnqueuedItem = newItem;
                snake.MapColor[snake.JustEnqueuedItem.Item1, snake.JustEnqueuedItem.Item2] = Color.Green;
                snake.Position.Enqueue(newItem);
                score++;
                richTextBox2.Text = score.ToString();

                snake.generateFood();
                return;
            }

            snake.JustEnqueuedItem = newItem;
            snake.Position.Enqueue(newItem);
            snake.MapColor[snake.JustEnqueuedItem.Item1, snake.JustEnqueuedItem.Item2] = Color.Green;
            newItem = snake.Position.Peek();
            snake.MapColor[newItem.Item1, newItem.Item2] = Color.White;
            snake.Position.Dequeue();
            keyTable.Clear();

            tableLayoutPanel1.Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SnakeInstance.GridSize; i++)
                for (int j = 0; j < SnakeInstance.GridSize; j++)
                    snake.MapColor[i, j] = Color.White;

            startGame = true;
            difficultySelected = false;

            button2.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            richTextBox1.Visible = false;
            richTextBox2.Visible = false;
            richTextBox3.Visible = false;
            richTextBox4.Visible = false;
            difficultySelected = false;
            richTextBox5.Visible = false;
            button1.Visible = false;

            snake = new SnakeInstance("RIGHT");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            snake.SnakeSpeed = 200;
            timer1.Interval = snake.SnakeSpeed;
            button2.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            richTextBox1.Visible = true;
            richTextBox2.Visible = true;
            richTextBox3.Visible = true;
            richTextBox4.Visible = true;
            difficultySelected = true;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            snake.SnakeSpeed = 100;
            timer1.Interval = snake.SnakeSpeed;
            button2.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            richTextBox1.Visible = true;
            richTextBox2.Visible = true;
            richTextBox3.Visible = true;
            richTextBox4.Visible = true;
            difficultySelected = true;
        }

        private void Button4_Click(object sender, EventArgs e)
        {

            snake.SnakeSpeed = 50;
            timer1.Interval = snake.SnakeSpeed;
            button2.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            richTextBox1.Visible = true;
            richTextBox2.Visible = true;
            richTextBox3.Visible = true;
            richTextBox4.Visible = true;
            difficultySelected = true;
        }
    }

    public class SnakeInstance
    {
        // The size of the game board
        private static int GRID_SIZE = 30;
        // The map of the whole game,
        private Color[,] mapColor;
        // Speed of the snake and basically the difficulty of the game
        private int SNAKE_SPEED;
        // 0 for up // 1 for right // 2 for down // 3 for left
        private int direction;
        // Initial direction of the snake, same codification as direction
        private int INITIAL_DIRECTION;
        // The item that was just enqueued (a peek that works on the other side)
        private Tuple<int, int> justEnqueuedItem;
        // Queue with all of the snake's blocks
        private Queue<Tuple<int, int>> position;

        // default constructor
        public SnakeInstance()
        {
            position = new Queue<Tuple<int, int>>();
        }

        public SnakeInstance(string INITIAL_DIRECTION)
        {
            switch (INITIAL_DIRECTION)
            {
                case "LEFT":
                    this.INITIAL_DIRECTION = 3;
                    break;
                case "UP":
                    this.INITIAL_DIRECTION = 0;
                    break;
                case "RIGHT":
                    this.INITIAL_DIRECTION = 1;
                    break;
                case "DOWN":
                    this.INITIAL_DIRECTION = 2;
                    break;
                default:
                    throw new ArgumentException("Invalid direction: ", INITIAL_DIRECTION);
            }

            GRID_SIZE = 30;
            position = new Queue<Tuple<int, int>>();
            mapColor = new Color[GRID_SIZE, GRID_SIZE];
            initilizeSnake(mapColor);

            SnakeSpeed = SNAKE_SPEED;
            Direction = direction;
            JustEnqueuedItem = justEnqueuedItem;
            Position = position;
            MapColor = mapColor;
            GridSize = GRID_SIZE;
        }

        // getters and setters

        public static int GridSize { get; set; }

        public int SnakeSpeed { get; set; }

        public int Direction { get; set; }

        public Tuple<int, int> JustEnqueuedItem { get; set; }

        public Queue<Tuple<int, int>> Position { get; set; }

        public Color[,] MapColor { get; set; }

        private void initilizeSnake(Color[,] color)
        {

            // we put the snake in a random position !!!!!!!!
            Tuple<int, int> point1 = new Tuple<int, int>(2, 14);
            direction = INITIAL_DIRECTION;

            position.Enqueue(point1);
            color[point1.Item1, point1.Item2] = Color.Green;

            Tuple<int, int> point2 = new Tuple<int, int>(point1.Item1 + 1, point1.Item2);
            position.Enqueue(point2);
            color[point2.Item1, point2.Item2] = Color.Green;

            Tuple<int, int> point3 = new Tuple<int, int>(point2.Item1 + 1, point2.Item2);
            position.Enqueue(point3);
            color[point3.Item1, point3.Item2] = Color.Green;

            Tuple<int, int> point4 = new Tuple<int, int>(point3.Item1 + 1, point3.Item2);
            position.Enqueue(point4);
            color[point4.Item1, point4.Item2] = Color.Green;
            justEnqueuedItem = point4;

            generateFood();
        }



        public void generateFood()
        {
            Random randomX = new Random();
            int randomNumberX = randomX.Next(0, GRID_SIZE);
            Random randomY = new Random();
            int randomNumberY = randomY.Next(0, GRID_SIZE);

            while (mapColor[randomNumberX, randomNumberY] == Color.Green)
            {
                randomX = new Random();
                randomNumberX = randomX.Next(0, GRID_SIZE);
                randomY = new Random();
                randomNumberY = randomY.Next(0, GRID_SIZE);
            }

            mapColor[randomNumberX, randomNumberY] = Color.Red;
        }

    }
}