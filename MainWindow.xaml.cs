using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphColoring
{
    public partial class MainWindow : Window
    {
        private int[,] graph; // Матрица смежности
        private int[] colors; // Массив цветов вершин
        private int verticesCount; // Количество вершин
        private List<Ellipse> vertices = new(); // Список вершин (UI)
        private List<Line> edges = new(); // Список рёбер (UI)
        private Random random = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateGraphButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(VertexCountInput.Text, out verticesCount) || verticesCount < 1)
            {
                MessageBox.Show("Введите корректное количество вершин!");
                return;
            }

            ResetGraph();
            GenerateRandomGraph(verticesCount);
            DrawGraph();
            ColorGraphButton.IsEnabled = true;
        }
        private void VertexCountInput_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Введите кол-во вершин")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void VertexCountInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Введите кол-во вершин";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void ColorGraphButton_Click(object sender, RoutedEventArgs e)
        {
            int minColors = FindMinColors();
            MessageBox.Show($"Минимальное количество цветов: {minColors}");
            DrawGraph(); // Перерисовать граф с раскрашенными вершинами
        }

        private void ResetGraph()
        {
            GraphCanvas.Children.Clear();
            vertices.Clear();
            edges.Clear();
        }

        private void GenerateRandomGraph(int count)
        {
            graph = new int[count, count];
            colors = new int[count];

            // Генерация случайного графа
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    graph[i, j] = graph[j, i] = random.Next(0, 2); // Случайное соединение (0 или 1)
                }
            }
        }

        private void DrawGraph()
        {
            ResetGraph();
            const int radius = 20;
            const int canvasSize = 400;
            const int graphRadius = 150;

            Point center = new(canvasSize / 2, canvasSize / 2);

            // Расположение вершин по кругу
            for (int i = 0; i < verticesCount; i++)
            {
                double angle = 2 * Math.PI * i / verticesCount;
                double x = center.X + graphRadius * Math.Cos(angle);
                double y = center.Y + graphRadius * Math.Sin(angle);

                Ellipse vertex = new()
                {
                    Width = radius,
                    Height = radius,
                    Fill = colors[i] == 0 ? Brushes.LightGray : GetColorBrush(colors[i]),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Tag = i
                };

                Canvas.SetLeft(vertex, x - radius / 2);
                Canvas.SetTop(vertex, y - radius / 2);
                GraphCanvas.Children.Add(vertex);
                vertices.Add(vertex);

                // Нумерация вершин
                TextBlock text = new()
                {
                    Text = (i + 1).ToString(),
                    Foreground = Brushes.Black,
                    FontSize = 12
                };
                Canvas.SetLeft(text, x - 5);
                Canvas.SetTop(text, y - 10);
                GraphCanvas.Children.Add(text);
            }

            // Рисование рёбер
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = i + 1; j < verticesCount; j++)
                {
                    if (graph[i, j] == 1)
                    {
                        Line edge = new()
                        {
                            X1 = Canvas.GetLeft(vertices[i]) + radius / 2,
                            Y1 = Canvas.GetTop(vertices[i]) + radius / 2,
                            X2 = Canvas.GetLeft(vertices[j]) + radius / 2,
                            Y2 = Canvas.GetTop(vertices[j]) + radius / 2,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        };
                        GraphCanvas.Children.Add(edge);
                        edges.Add(edge);
                    }
                }
            }
        }

        private int FindMinColors()
        {
            int colorCount = 1;
            while (true)
            {
                if (ColorGraph(0, colorCount))
                    return colorCount;
                colorCount++;
            }
        }

        private bool ColorGraph(int vertex, int maxColors)
        {
            if (vertex == verticesCount) // Все вершины раскрашены
                return true;

            for (int color = 1; color <= maxColors; color++)
            {
                if (IsSafe(vertex, color))
                {
                    colors[vertex] = color; // Присваиваем вершине цвет

                    if (ColorGraph(vertex + 1, maxColors)) // Рекурсивно раскрашиваем следующую вершину
                        return true;

                    colors[vertex] = 0; // Отмена, если решение не найдено
                }
            }

            return false; // Если ни один цвет не подошел
        }

        private bool IsSafe(int vertex, int color)
        {
            for (int i = 0; i < verticesCount; i++)
            {
                if (graph[vertex, i] == 1 && colors[i] == color) // Если сосед имеет тот же цвет
                    return false;
            }
            return true;
        }

        private Brush GetColorBrush(int color)
        {
            return color switch
            {
                1 => Brushes.Red,
                2 => Brushes.Green,
                3 => Brushes.Blue,
                4 => Brushes.Yellow,
                5 => Brushes.Pink,
                6 => Brushes.DarkSeaGreen,
                7 => Brushes.DarkRed,
                8 => Brushes.DarkMagenta,
                9 => Brushes.DarkOrange,
                10 => Brushes.Orange,
                _ => Brushes.Gray,
            };
        }
    }
}