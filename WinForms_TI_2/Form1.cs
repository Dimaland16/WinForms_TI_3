using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinForms_TI_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int MAX_LENGTH = 20;
        byte[] messageBytes;
        byte[] keyBytes;
        byte[] resultBytes;
        ulong g;
        ulong p;
        ulong x;
        ulong k;
        ulong y;
        List<ulong> c;



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int currentCharacters = textBox1.Text.Length;

            if (currentCharacters > MAX_LENGTH)
            {
                textBox1.Text = textBox1.Text[..MAX_LENGTH];
                textBox1.SelectionStart = MAX_LENGTH;
            }
            else
            {
                label1.Text = $"{currentCharacters}";
            }

            if (currentCharacters > 0)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }

        }

        private bool IsPrime(ulong number)
        {
            if (number <= 1)
            {
                return false;
            }
            if (number == 2)
            {
                return true;
            }
            if (number % 2 == 0)
            {
                return false;
            }

            ulong boundary = (ulong)Math.Floor(Math.Sqrt(number));
            for (ulong i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void OutputToListBox(List<ulong> list)
        {
            listBox1.Items.Clear();

            foreach (ulong item in list)
            {
                listBox1.Items.Add(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (ulong.TryParse(textBox1.Text, out p))
            {
                if (IsPrime(p))
                {
                    MessageBox.Show("Число простое.");
                    List<ulong> primitiveRoots = FindPrimitiveRoots(p);
                    OutputToListBox(primitiveRoots);

                }
                else
                {
                    MessageBox.Show("Число не является простым, введите простое число!");
                    textBox1.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Некорректный ввод.");
            }

        }

        // Метод для нахождения всех первообразных корней по модулю p
        public static List<ulong> FindPrimitiveRoots(ulong p)
        {
            List<ulong> primitiveRoots = new List<ulong>();
            List<ulong> primeFactors = GetPrimeFactors(p - 1);

            for (ulong g = 2; g < p; g++)
            {
                bool isPrimitiveRoot = true;

                foreach (ulong primeFactor in primeFactors)
                {
                    if (ModularExponentiation(g, (p - 1) / primeFactor, p) == 1)
                    {
                        isPrimitiveRoot = false;
                        break;
                    }
                }

                if (isPrimitiveRoot)
                {
                    primitiveRoots.Add(g);
                }
            }

            return primitiveRoots;
        }

        // Метод для получения простых делителей числа n
        private static List<ulong> GetPrimeFactors(ulong n)
        {
            List<ulong> primeFactors = new List<ulong>();
            for (ulong i = 2; i * i <= n; i++)
            {
                while (n % i == 0)
                {
                    primeFactors.Add(i);
                    n /= i;
                }
            }
            if (n > 1)
            {
                primeFactors.Add(n);
            }
            return primeFactors;
        }

        // Метод для быстрого возведения в степень по модулю
        private static ulong ModularExponentiation(ulong a, ulong b, ulong n)
        {
            ulong a1 = a;
            ulong b1 = b;
            ulong x = 1;

            while (b1 != 0)
            {
                while (b1 % 2 == 0)
                {
                    b1 /= 2;
                    a1 = (a1 * a1) % n;
                }

                b1--;
                x = (x * a1) % n;
            }

            return x;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool next = true;
            if (textBox2.Text.Length != 0 && textBox3.Text.Length != 0)
            {
                if (ulong.TryParse(textBox2.Text, out ulong x))
                {
                    if (x <= 1 || x >= (p - 1))
                    {
                        MessageBox.Show("Некорректное значение x! Значение x должно быть в диапазоне 1 < x < p - 1");
                        next = false;
                    }
                    else
                        this.x = x;
                }
                else
                {
                    MessageBox.Show("Некорректное значение x.");
                    next = false;
                }

                if (ulong.TryParse(textBox3.Text, out ulong k))
                {

                    if (k <= 1 || k >= (p - 1) || !AreCoprime(p - 1, k))
                    {
                        MessageBox.Show("Некорректное значение k! Значение k должно быть в диапазоне 1 < k < p - 1 и взаимно простым с числом p - 1");
                        next = false;
                    }
                    else
                        this.k = k;
                }
                else
                {
                    MessageBox.Show("Некорректное значение k.");
                    next = false;
                }
            }
            else
            {
                MessageBox.Show("Числа x и k не могут быть пустыми!");
                next = false;
            }

            if (next) 
            {
                y = ModularExponentiation(g, x, p);
                c = Encrypt();
                foreach (ulong num in c)
                {
                    richTextBox2.AppendText(num.ToString("D") + " ");
                }
            }
        }


        static ulong GCD(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static bool AreCoprime(ulong a, ulong b)
        {
            return GCD(a, b) == 1;
        }

        // Метод для шифрования
        public List<ulong> Encrypt()
        {
            List<ulong> encrypted = new List<ulong>();

            foreach (byte byt in messageBytes)
            {

                ulong a = ModularExponentiation(g, k, p);
                ulong b = ModularExponentiation(y, k, p) * byt % p;

                encrypted.Add(a);
                encrypted.Add(b);
            }

            return encrypted;
        }

        // Метод для дешифрования
        public string Decrypt()
        {
            byte[] decryptedBytes = new byte[c.Count / 2];

            for (int i = 0; i < c.Count; i += 2)
            {
                ulong a = c[i];
                ulong b = c[i + 1];

                // Вычисляем m
                ulong m = b * ModularExponentiation(a, p - 1 - x, p) % p;

                decryptedBytes[i / 2] = (byte)m;
            }

            return System.Text.Encoding.UTF8.GetString(decryptedBytes);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    messageBytes = File.ReadAllBytes(filePath);
                    string binaryRepresentation = "";
                    foreach (byte b in messageBytes)
                    {
                        binaryRepresentation += Convert.ToString(b, 2).PadLeft(8, '0');
                    }
                    richTextBox3.Text = binaryRepresentation;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary Files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            resultBytes = new byte[messageBytes.Length];


            for (int i = 0; i < richTextBox2.Text.Length; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                resultBytes[byteIndex] |= (byte)((richTextBox2.Text[i] - '0') << (7 - bitIndex));
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(saveFileDialog.FileName, resultBytes);

                    MessageBox.Show("Массив байтов успешно сохранен в файл " + saveFileDialog.FileName,
                                    "Сохранение завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении массива байтов в файл: " + ex.Message,
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (keyBytes.Length != resultBytes.Length)
            {
                Console.WriteLine("Длины массивов не совпадают, операция XOR не может быть выполнена.");
                return;
            }

            for (int i = 0; i < keyBytes.Length; i++)
            {
                messageBytes[i] = (byte)(keyBytes[i] ^ resultBytes[i]);
            }

            string messageString = "";

            foreach (byte b in messageBytes)
            {
                messageString += Convert.ToString(b, 2).PadLeft(8, '0');
            }

            richTextBox2.Text = messageString;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                g = (ulong)listBox1.SelectedItem;

            }
        }
    }
}
