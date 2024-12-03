// Form1.cs
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TicTacToeGame
{
    public partial class Form1 : Form
    {
        private Button[,] buttons = new Button[3, 3];
        private bool isPlayerXTurn;
        private RadioButton easyModeButton;
        private RadioButton hardModeButton;
        private CheckBox firstMoveCheckbox;
        private Button newGameButton;
        private MenuStrip menuStrip;
        private ToolStrip toolStrip;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(400, 600); // Увеличен размер формы для размещения элементов
            this.Text = "Крестики-нолики";

            // Создание главного меню
            menuStrip = new MenuStrip();
            var gameMenu = new ToolStripMenuItem("Игра");
            var newGameMenuItem = new ToolStripMenuItem("Новая игра", null, NewGameButton_Click);
            var exitMenuItem = new ToolStripMenuItem("Выход", null, (s, e) => Application.Exit());
            gameMenu.DropDownItems.Add(newGameMenuItem);
            gameMenu.DropDownItems.Add(exitMenuItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            var aboutMenuItem = new ToolStripMenuItem("О программе", null, (s, e) => MessageBox.Show("Крестики-нолики v1.0\nАвтор: Ямчук Тимур", "О программе"));
            helpMenu.DropDownItems.Add(aboutMenuItem);

            menuStrip.Items.Add(gameMenu);
            menuStrip.Items.Add(helpMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Создание панели инструментов
            toolStrip = new ToolStrip();
            var newGameToolButton = new ToolStripButton("Новая игра", null, NewGameButton_Click, "NewGame");
            var exitToolButton = new ToolStripButton("Выход", null, (s, e) => Application.Exit(), "Exit");
            toolStrip.Items.Add(newGameToolButton);
            toolStrip.Items.Add(exitToolButton);
            this.Controls.Add(toolStrip);

            // Размещение игрового поля
            int buttonSize = 100;
            int spacing = 10; // Расстояние между кнопками
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j] = new Button
                    {
                        Size = new Size(buttonSize, buttonSize),
                        Font = new Font(FontFamily.GenericSansSerif, 24),
                        Text = ""
                    };
                    buttons[i, j].Location = new Point(j * (buttonSize + spacing) + 50, i * (buttonSize + spacing) + 100);
                    buttons[i, j].Click += Button_Click;
                    this.Controls.Add(buttons[i, j]);
                }
            }

            // Размещение элементов управления
            firstMoveCheckbox = new CheckBox
            {
                Location = new Point(50, 450),
                Text = "Первый ход за X",
                Checked = true,
                AutoSize = true 
            };

            this.Controls.Add(firstMoveCheckbox);

            easyModeButton = new RadioButton
            {
                Location = new Point(50, 475),
                Text = "Легкий уровень",
                Checked = true
            };
            this.Controls.Add(easyModeButton);

            hardModeButton = new RadioButton
            {
                Location = new Point(50, 500),
                Text = "Сложный уровень"
            };
            this.Controls.Add(hardModeButton);

            newGameButton = new Button
            {
                Location = new Point(300, 450),
                Size = new Size(75, 50),
                Text = "Новая игра"
            };
            newGameButton.Click += NewGameButton_Click;
            this.Controls.Add(newGameButton);

            InitializeGame();
        }

        private void InitializeGame()
        {
            isPlayerXTurn = firstMoveCheckbox.Checked; // Определяем, кто ходит первым

            foreach (var button in buttons)
            {
                button.Text = "";
                button.Enabled = true;
                button.BackColor = SystemColors.Control;
            }

            if (!isPlayerXTurn)
            {
                MakeAIMove();
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && string.IsNullOrEmpty(clickedButton.Text))
            {
                clickedButton.Text = isPlayerXTurn ? "X" : "O";
                clickedButton.Enabled = false;

                if (CheckWinCondition())
                {
                    MessageBox.Show($"{(isPlayerXTurn ? "X" : "O")} wins!", "Game Over");
                    DisableAllButtons();
                    return;
                }

                if (CheckDrawCondition())
                {
                    MessageBox.Show("Draw!", "Game Over");
                    return;
                }

                isPlayerXTurn = !isPlayerXTurn;

                if (!isPlayerXTurn)
                {
                    MakeAIMove();
                }
            }
        }

        private void MakeAIMove()
        {
            if (easyModeButton.Checked)
            {
                MakeRandomMove();
            }
            else if (hardModeButton.Checked)
            {
                MakeOptimalMove();
            }

            if (CheckWinCondition())
            {
                MessageBox.Show("O wins!", "Game Over");
                DisableAllButtons();
                return;
            }

            if (CheckDrawCondition())
            {
                MessageBox.Show("Draw!", "Game Over");
                return;
            }

            isPlayerXTurn = true; // После хода AI передаем очередь игроку
        }

        private void MakeRandomMove()
        {
            var emptyButtons = buttons.Cast<Button>().Where(b => string.IsNullOrEmpty(b.Text)).ToList();
            if (emptyButtons.Any())
            {
                var random = new Random();
                var button = emptyButtons[random.Next(emptyButtons.Count)];
                button.Text = "O";
                button.Enabled = false;
            }
        }

        private void MakeOptimalMove()
        {
            for (int i = 0; i < 3; i++)
            {
                if (TryCompleteLine(buttons[i, 0], buttons[i, 1], buttons[i, 2])) return;
                if (TryCompleteLine(buttons[0, i], buttons[1, i], buttons[2, i])) return;
            }

            if (TryCompleteLine(buttons[0, 0], buttons[1, 1], buttons[2, 2])) return;
            if (TryCompleteLine(buttons[0, 2], buttons[1, 1], buttons[2, 0])) return;

            MakeRandomMove();
        }

        private bool TryCompleteLine(Button b1, Button b2, Button b3)
        {
            if (b1.Text == "O" && b2.Text == "O" && string.IsNullOrEmpty(b3.Text))
            {
                b3.Text = "O";
                b3.Enabled = false;
                return true;
            }

            if (b1.Text == "O" && b3.Text == "O" && string.IsNullOrEmpty(b2.Text))
            {
                b2.Text = "O";
                b2.Enabled = false;
                return true;
            }

            if (b2.Text == "O" && b3.Text == "O" && string.IsNullOrEmpty(b1.Text))
            {
                b1.Text = "O";
                b1.Enabled = false;
                return true;
            }

            if (b1.Text == "X" && b2.Text == "X" && string.IsNullOrEmpty(b3.Text))
            {
                b3.Text = "O";
                b3.Enabled = false;
                return true;
            }

            if (b1.Text == "X" && b3.Text == "X" && string.IsNullOrEmpty(b2.Text))
            {
                b2.Text = "O";
                b2.Enabled = false;
                return true;
            }

            if (b2.Text == "X" && b3.Text == "X" && string.IsNullOrEmpty(b1.Text))
            {
                b1.Text = "O";
                b1.Enabled = false;
                return true;
            }

            return false;
        }

        private bool CheckWinCondition()
        {
            for (int i = 0; i < 3; i++)
            {
                if (CheckLine(buttons[i, 0], buttons[i, 1], buttons[i, 2]) || CheckLine(buttons[0, i], buttons[1, i], buttons[2, i]))
                {
                    return true;
                }
            }

            if (CheckLine(buttons[0, 0], buttons[1, 1], buttons[2, 2]) || CheckLine(buttons[0, 2], buttons[1, 1], buttons[2, 0]))
            {
                return true;
            }

            return false;
        }

        private bool CheckLine(Button b1, Button b2, Button b3)
        {
            return b1.Text == b2.Text && b2.Text == b3.Text && !string.IsNullOrEmpty(b1.Text);
        }

        private bool CheckDrawCondition()
        {
            return buttons.Cast<Button>().All(b => !string.IsNullOrEmpty(b.Text));
        }

        private void DisableAllButtons()
        {
            foreach (var button in buttons)
            {
                button.Enabled = false;
            }
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            InitializeGame();
        }
    }
}
