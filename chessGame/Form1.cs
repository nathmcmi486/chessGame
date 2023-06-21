namespace chessGame
{
    using System.Media;
    public partial class Form1 : Form
    {

        List<Rectangle> boardSquare = new List<Rectangle>();
        List<List<string>> board = new List<List<string>>();
        const int SQUARE_SIZE = 60;
        string currentState = "starting";
        List<Rectangle> menuButtons = new List<Rectangle>();
        bool playerVPlayer = false;
        bool firstComputerMove = true;

        int mouseXPos = 0;
        int mouseYPos = 0;

        int selectedSquare = -1;
        string selectedPeice = "";
        bool whiteMove = true;
        bool whiteCheck = false;
        bool blackCheck = false;

        int computerX = -1;
        int computerY = -1;
        string computerProtects = "";

        const int DEFAULT_PAWN = 1;
        const int DEFAULT_KNIGHT = 2;
        const int DEFAULT_KING = 2;
        const int DEFAULT_BISHOP = 3;
        const int DEFAULT_ROOK = 4;
        const int DEFAULT_QUEEN = 5;

        public Form1()
        {
            InitializeComponent();

            Random randomGen = new Random();
            int chance = randomGen.Next(0, 101);

            // Just for fun
            if (chance < 5)
            {
                whiteMove = false;
            }

            int startingX = 10;
            int startingY = 10;

            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    startingY += SQUARE_SIZE;
                    startingX = 10;
                }

                startingX += SQUARE_SIZE;

                boardSquare.Add(new Rectangle(startingX, startingY, SQUARE_SIZE, SQUARE_SIZE));
            }

            for (int i = 0; i < 8; i++)
            {
                board.Add(new List<string>());

                for (int j = 0; j < 8; j++)
                {
                    board[i].Add("e");
                }
            }

            board[0][0] = "r1B";
            board[1][0] = "n1B";
            board[2][0] = "b1B";
            board[3][0] = "qB";
            board[4][0] = "kB";
            board[5][0] = "b2B";
            board[6][0] = "n2B";
            board[7][0] = "r2B";

            for (int i = 0; i < 8; i++)
            {
                board[i][1] = $"p{i + 1}B";
            }

            board[0][7] = "r1W";
            board[1][7] = "n1W";
            board[2][7] = "b1W";
            board[3][7] = "qW";
            board[4][7] = "kW";
            board[5][7] = "b2W";
            board[6][7] = "n2W";
            board[7][7] = "r2W";

            for (int i = 0; i < 8; i++)
            {
                board[i][6] = $"p{i + 1}W";
            }

            // Add menu buttons
            for (int i = 0; i <= 3; i++)
            {
                menuButtons.Add(new Rectangle(100 + (i * 165), 50, 150, 50));
            }

            if (playerVPlayer == false)
            {
                whiteMove = true;
            }
        }

        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        SolidBrush lightGrayBrush = new SolidBrush(Color.DarkGray);

        private int[] getPeicePos(string peiceName)
        {
            int[] ret = new int[2];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[x][y] == peiceName)
                    {
                        ret[0] = x;
                        ret[1] = y;
                        break;
                    }
                }
            }

            return ret;
        }

        private bool validDiagonal(int x, int y, int moveX, int moveY)
        {
            int xDiff = x - moveX;
            int yDiff = y - moveY;

            // If the difference in x and y is the same then it should be moving diagonally
            if (Math.Abs(xDiff) != Math.Abs(yDiff))
            {
                return false;
            }

            int checkX = x;
            int checkY = y;
            
            while (checkX != moveX && checkY != moveY) {
                if (x > moveX)
                {
                    checkX--;
                } else { checkX++; }

                if (y > moveY)
                {
                    checkY--;
                } else { checkY++; }

                if (board[checkX][checkY] != "e")
                {
                    if (checkX == moveX && checkY == moveY)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }

        private bool validStraight(int x, int y, int moveX, int moveY)
        {
            if (moveX == x)
            {
                if (moveY < y)
                {
                    for (int checkY = y - 1; checkY > moveY; checkY--)
                    {
                        if (board[moveX][checkY] != "e")
                        {
                            return false;
                        }
                    }

                    return true;
                } else if (moveY > y)
                {
                    for (int checkY = y + 1; checkY < moveY; checkY++)
                    {
                        if (board[moveX][checkY] != "e")
                        {
                            return false;
                        }
                    }

                    return true;
                }
            } else if (moveY == y)
            {
                if (moveX < x)
                {
                    for (int checkX = x - 1; checkX > moveX; checkX--)
                    {
                        if (board[checkX][moveY] != "e")
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else if (moveX > x)
                {
                    for (int checkX = x + 1; checkX < moveX; checkX++)
                    {
                        if (board[checkX][moveY] != "e")
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        // king = "W" for checking if white king is in check
        // king = "B" for checking if black king is in check
        private void checkForCheck(string king)
        {
            // this is horrific
            for (int peiceY = 0; peiceY < 8; peiceY++)
            {
                for (int peiceX = 0; peiceX < 8; peiceX++)
                {
                    if (board[peiceX][peiceY].Contains("W") && king == "W")
                    {
                        continue;
                    } else if (board[peiceX][peiceY].Contains("B") && king == "B")
                    {
                        continue;
                    }

                    for (int positionY = 0; positionY < 8; positionY++)
                    {
                        for (int positionX = 0; positionX < 8; positionX++)
                        {
                            if (board[positionX][positionY] == $"k{king}" && checkValidMove(board[peiceX][peiceY], positionX, positionY) == true)
                            {
                                if (king == "W")
                                {
                                    whiteCheck = true;
                                    return;
                                } else { 
                                    blackCheck = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            whiteCheck = false;
            blackCheck = false;
        }

        private bool checkValidMove(string peiceName, int moveX, int moveY)
        {
            /*if (board[moveX][moveY].Contains("W") && peiceName.Contains("W") || board[moveX][moveY].Contains("B") && peiceName.Contains("B"))
            {
                return false;
            }*/

            if (peiceName.Contains("W") && board[moveX][moveY].Contains("W") || peiceName.Contains("B") && board[moveX][moveY].Contains("B"))
            {
                return false;
            }

            int[] peicePos = getPeicePos(peiceName);
            int x = peicePos[0];
            int y = peicePos[1];

            //return true;

           // this.currentMoveInfoLabel.Text = $"{peiceName} {x} {y} - {moveX} {moveY}";

            if (peiceName.StartsWith("p")) {
                // White pawn
                if (peiceName.Contains("W"))
                {
                    if (Math.Abs(moveX - x) == 1 && moveY == y - 1)
                    {
                        if (board[moveX][moveY].Contains("B"))
                        {
                            return true;
                        } else { return false; }
                    }

                    if (board[moveX][moveY] != "e")
                    {
                        return false;
                    }

                    return moveY == y - 1;
                }
                else if (peiceName.Contains("B"))
                {
                    if (Math.Abs(moveX - x) == 1 && moveY == y + 1)
                    {
                        if (board[moveX][moveY].Contains("W"))
                        {
                            return true;
                        }
                        else { return false; }
                    }

                    if (board[moveX][moveY] != "e")
                    {
                        return false;
                    }

                    return moveY == y + 1;
                } else { return false; }
            }

            // Rooks
            if (peiceName.StartsWith("r"))
            {
                return validStraight(x, y, moveX, moveY);
            }

            // Bishops
            if (peiceName.StartsWith("b"))
            {
                return validDiagonal(x, y, moveX, moveY);
            }

            // Queens
            if (peiceName.StartsWith("q"))
            {
                // These parts should be their own methods
                // Check if it’s moving diagonally (like bishops)
                if (validStraight(x, y, moveX, moveY) || validDiagonal(x, y, moveX, moveY))
                {
                    return true;
                } else { return false; }
            }

            // Kings
            if (peiceName.StartsWith("k"))
            {
                int xDiff = x - moveX;
                int yDiff = y - moveY;

                if (Math.Abs(xDiff) <= 1 && Math.Abs(yDiff) <= 1)
                {
                    return true;
                }
            }

            // Knights
            if (peiceName.StartsWith("n"))
            {
                int xDiff = x - moveX;
                int yDiff = y - moveY;

                if (Math.Abs(xDiff) == 1 && Math.Abs(yDiff) == 2 || Math.Abs(xDiff) == 2 && Math.Abs(yDiff) == 1)
                {
                    return true;
                }
            }


            return false;
        }

        private void movePeice()
        {
            int squareN = 0;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (playerVPlayer == false && whiteMove == false && x == computerX && y == computerY)
                    {
                        int[] ogPos = getPeicePos(selectedPeice);
                        board[ogPos[0]][ogPos[1]] = "e";
                        board[x][y] = selectedPeice;
                        return;
                    }

                    if (squareN == selectedSquare)
                    {
                        // Make a sound when the piece moves
                        //System.Media.SoundPlayer sound = new System.Media.SoundPlayer(Properties.Resources.wood_block);
                        // sound.Play();
                        int[] ogPos = getPeicePos(selectedPeice);
                        board[ogPos[0]][ogPos[1]] = "e";
                        board[x][y] = selectedPeice;
                        return;
                    }

                    squareN++;
                }
            }
        }

        private void computerMove()
        {
            Random rand = new Random();
            List<string> movePieceName = new List<string>();
            List<int[]> moveValues = new List<int[]>();
            List<List<string>> tmpBoard = new List<List<string>>();

            bool randomMove = true;

            if (rand.Next(0, 101) > 50)
            {
                randomMove = false;
            }

            if (firstComputerMove)
            {
                List<int[]> movePieces = new List<int[]>();

                bool validComputerMove = false;
                int rndMX = 0;
                int rndMY = 0;

                int checkedPieces = 0;

                while (validComputerMove == false)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            if (board[x][y].Contains("B") == false)
                            {
                                continue;
                            }

                            // The piece has already been looked at and it is not a random move
                            if (randomMove == false && movePieceName.Contains(board[x][y]))
                            {
                                continue;
                            }

                            int attempts = 0;

                            while (validComputerMove == false)
                            {
                                if (attempts == 128)
                                {
                                    break;
                                }

                                if (board[x][y].StartsWith("p") || board[x][y].StartsWith("k"))
                                {
                                    int xAdd = 0;
                                    int yAdd = 0;

                                    if (x >= 2 && x <= 6)
                                    {
                                        xAdd = 1;
                                    }

                                    if (y >= 2 && y <= 6)
                                    {
                                        yAdd = 1;
                                    }

                                    rndMX = rand.Next(x - xAdd, x + xAdd);
                                    rndMY = rand.Next(y - yAdd, y + yAdd);
                                }
                                else
                                {
                                    rndMX = rand.Next(0, 7);
                                    rndMY = rand.Next(0, 7);
                                }
                                validComputerMove = checkValidMove(board[x][y], rndMX, rndMY);
                                attempts++;
                            }

                            if (randomMove == false && validComputerMove == true)
                            {
                                movePieceName.Add(board[x][y]);
                                moveValues.Add(new int[] { x, y });
                                validComputerMove = false;
                                checkedPieces += 1;
                            } else if (validComputerMove == true)
                            {
                                selectedPeice = board[x][y];
                                selectedSquare = rndMX + (rndMY * 8);
                                break;
                            } else if (checkedPieces == 5)
                            {
                                break;
                            }
                        }

                        if (validComputerMove == true)
                        {
                            break;
                        }
                    }
                }

                if (randomMove)
                {
                    movePeice();
                } else
                {

                }
                return;
            }
        }

        private void mainLoop(object _o, EventArgs _e)
        {
            Rectangle mouseRect = new Rectangle(mouseXPos, mouseYPos, 5, 5);

            if (currentState == "starting")
            {
                if (mouseRect.IntersectsWith(menuButtons[0]))
                {
                    playerVPlayer = true;
                    currentState = "game";
                } else if (mouseRect.IntersectsWith(menuButtons[1]))
                {
                    playerVPlayer = false;
                    currentState = "game";
                } else if (mouseRect.IntersectsWith(menuButtons[2]))
                {
                    System.Windows.Forms.Application.Exit();
                }

                this.Refresh();
                return;
            }

            int sqaureN = 0;

            if (whiteMove)
            {
                this.currentMoveInfoLabel.Text = "White's move";
            } else { this.currentMoveInfoLabel.Text = "Black's move"; }

            if (whiteMove == false && playerVPlayer == false)
            {
                computerMove();
                whiteMove = true;
                selectedPeice = "";
                selectedSquare = -1;
                this.Refresh();
                System.Threading.Thread.Sleep(1000);
                return;
            }

            // Check for selected peice[s]
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (mouseRect.IntersectsWith(boardSquare[sqaureN]))
                    {
                        if (selectedPeice == "")
                        {
                            selectedPeice = board[x][y];
                            this.currentMoveInfoLabel.Text = $"{selectedPeice} to: ";
                        } else
                        {
                            selectedSquare = sqaureN;
                            this.currentMoveInfoLabel.Text += $"{x}, {y}";
                        }
                    }

                    if (selectedPeice != "" && selectedSquare != -1)
                    {
                        bool isValid = checkValidMove(selectedPeice, x, y);

                        if (selectedPeice.Contains("W") && whiteMove != true)
                        {
                            isValid = false;
                        } else if (selectedPeice.Contains("B") && whiteMove == true)
                        {
                            isValid = false;
                        }

                        if (isValid == false)
                        {
                            this.Refresh();
                            this.currentMoveInfoLabel.Text = "Not a valid move";
                        }
                        else
                        {
                            movePeice();
                            checkForCheck("B");
                            checkForCheck("W");
                            whiteMove = !whiteMove;
                        }

                        selectedPeice = "";
                        selectedSquare = -1;
                    }

                    sqaureN++;
                }
            }

            if (whiteCheck)
            {
                this.inCheckLabel.Text = "White in check";
            }
            
            if (blackCheck)
            {
                this.inCheckLabel.Text = "Black in check";
            }

            mouseXPos = -1;
            mouseYPos = -1;

            this.Refresh();

            System.Threading.Thread.Sleep(500);
        }

        private void mouseHandler(object _o, MouseEventArgs e)
        {
            mouseXPos = e.X;
            mouseYPos = e.Y;
        }

        private void paintHandler(object _o, PaintEventArgs e)
        {
            Font pieceFont = new Font("Arial", 18);
            Font menuFont = new Font("Arial", 15);

            if (currentState == "starting")
            {
                e.Graphics.FillRectangle(whiteBrush, menuButtons[0]);
                e.Graphics.FillRectangle(blackBrush, menuButtons[1]);
                e.Graphics.FillRectangle(whiteBrush, menuButtons[2]);

                e.Graphics.DrawString("Player vs Player", menuFont, blackBrush, new Point(menuButtons[0].X, menuButtons[0].Y));
                e.Graphics.DrawString("Player vs computer", menuFont, whiteBrush, new Point(menuButtons[1].X, menuButtons[1].Y));
                e.Graphics.DrawString("Exit", menuFont, blackBrush, new Point(menuButtons[2].X, menuButtons[2].Y));

                return;
            }

            if (currentState == "game")
            {
                bool white = true;
                int sqaure = 0;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (sqaure % 8 == 0 && sqaure != 0)
                        {
                            white = !white;
                        }

                        if (white == true)
                        {
                            e.Graphics.FillRectangle(whiteBrush, boardSquare[sqaure]);
                            white = false;
                        }
                        else if (white == false && sqaure % 8 == 0)
                        {
                            e.Graphics.FillRectangle(blackBrush, boardSquare[sqaure]);
                            white = true;
                        }
                        else
                        {
                            e.Graphics.FillRectangle(blackBrush, boardSquare[sqaure]);
                            white = true;
                        }

                        if (board[x][y] != "e")
                        {
                            e.Graphics.DrawString(board[x][y], pieceFont, lightGrayBrush, new System.Drawing.Point(boardSquare[sqaure].X, boardSquare[sqaure].Y));
                        }

                        sqaure++;
                    }
                }
            }
        }
    }
}
