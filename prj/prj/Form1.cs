using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prj
{
    public partial class Form1 : Form
    {

        public double maxDailyUsage, maxDeliveryTime, avgDailyUsage, avgDeliveryTime, totalInventoryCost, allInventoryExpenses, leadTime, demand, annualDemand, orderCost, holdingCost;
        public double safetyStock, carryingCost, reorderPoint, economicOrderQuantity;

      

        public List<TextBox> textBoxes;
        public List<TextBox> textBoxesKey;
        public List<Label> labels;
        public List<Button> buttons;

        public class Item
        {
            public string Name { get; set; }
            public double MaxDailyUsage { get; set; }
            public double MaxDeliveryTime { get; set; }
            public double AvgDailyUsage { get; set; }
            public double AvgDeliveryTime { get; set; }
            public double TotalInventoryCost { get; set; }
            public double AllInventoryExpenses { get; set; }
            public double AnnualDemand { get; set; }
            public double OrderCost { get; set; }
            public double HoldingCost { get; set; }
            public double SafetyStock { get; set; }
            public double CarryingCost { get; set; }
            public double ReorderPoint { get; set; }
            public double EconomicOrderQuantity { get; set; }
        }

        List<Item> items = new List<Item>();

        // Добавяне на SaveFileDialog
        private SaveFileDialog saveFileDialog = new SaveFileDialog();

        string selectedCurrency;

        public Form1()
        {
            InitializeComponent();
            // Фиксиране на размера на формата
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            buttoncalculate.Visible = true;
            buttonExit.Visible = true;
            buttonRestart.Visible = false;
            buttonSaveData.Visible = false;
            buttonDownloadData.Visible = false;

            textBoxes = new List<TextBox>() { maxDailyUsageTextBox, maxDeliveryTimeTextBox, avgDailyUsageTextBox, avgDeliveryTimeTextBox, totalInventoryCostTextBox, allInventoryExpensesTextBox, annualDemandTextBox, orderCostTextBox, holdingCostTextBox };
            labels = new List<Label>() { safetyStockLabel, carryingCostLabel, reorderPointLabel, eoqLabel};
            buttons = new List<Button>() { buttonRestart, buttonExit, buttoncalculate , buttonSaveData, buttonDownloadData};
            textBoxesKey = new List<TextBox>() { nametextBox, unittextBox, maxDailyUsageTextBox, maxDeliveryTimeTextBox, avgDailyUsageTextBox, avgDeliveryTimeTextBox, totalInventoryCostTextBox,  allInventoryExpensesTextBox, annualDemandTextBox, orderCostTextBox, holdingCostTextBox };

            foreach(TextBox textBox in textBoxesKey)
            {
                textBox.TabStop = false;
            }

            foreach(Button button in buttons)
            {
                button.TabStop = false;
            }

            currencyComboBox.TabStop = false;

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    control.KeyDown += new KeyEventHandler(TextBox_KeyDown);
                }
            }

            // Задаване на филтъра за SaveFileDialog
            saveFileDialog.Filter = "Text Files|*.txt";

            selectedCurrency = "";

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                // Намиране на следващия или предишния TextBox
                int currentIndex = textBoxesKey.IndexOf(currentTextBox);
                int nextIndex = e.KeyCode == Keys.Down ? currentIndex + 1 : currentIndex - 1;
                if (nextIndex >= 0 && nextIndex < textBoxesKey.Count)
                {
                    // Преминаване към следващия или предишния TextBox
                    TextBox nextTextBox = textBoxesKey[nextIndex];
                    nextTextBox.Focus();
                }
                e.Handled = true;
            }
        }

        private void buttoncalculate_Click(object sender, EventArgs e)
        {
             foreach (TextBox textBox in textBoxes)
          {
              if (string.IsNullOrEmpty(textBox.Text))
              {
                  MessageBox.Show("Моля, попълнете всички полета.");
                  return;
              }
          }
           
            if (string.IsNullOrEmpty(nametextBox.Text) || string.IsNullOrEmpty(unittextBox.Text))
            {
                MessageBox.Show("Моля, попълнете всички полета.");
                return;
            }


          if (!double.TryParse(maxDailyUsageTextBox.Text, out maxDailyUsage) ||
              !double.TryParse(maxDeliveryTimeTextBox.Text, out maxDeliveryTime) ||
              !double.TryParse(avgDailyUsageTextBox.Text, out avgDailyUsage) ||
              !double.TryParse(avgDeliveryTimeTextBox.Text, out avgDeliveryTime) ||
              !double.TryParse(totalInventoryCostTextBox.Text, out totalInventoryCost) ||
              !double.TryParse(allInventoryExpensesTextBox.Text, out allInventoryExpenses) ||
              !double.TryParse(avgDeliveryTimeTextBox.Text, out leadTime) ||
              !double.TryParse(avgDailyUsageTextBox.Text, out demand) ||
              !double.TryParse(annualDemandTextBox.Text, out annualDemand) ||
              !double.TryParse(orderCostTextBox.Text, out orderCost) ||
              !double.TryParse(holdingCostTextBox.Text, out holdingCost))
          {
              MessageBox.Show("Моля, в полетата за Входни данни, въвеждайте само числа.");
              return;
          }

          double value;
          foreach (TextBox textBox in textBoxes)
          {
              if (double.TryParse(textBox.Text, out value))
              {
                  if (value <= 0)
                  {
                      MessageBox.Show("Моля, в полетата за Входни данни, въвеждайте числа, които са по-големи от 0.");
                      textBox.Focus();
                      return;
                  }
              }
          }

            if (!Regex.IsMatch(unittextBox.Text, @"^[a-zA-Zа-яА-Я\.]+$"))
            {
                MessageBox.Show("Моля, в полето за Мерна единица въвеждайте само текстови символи или точка. Например, ‘кг.’, ‘л.’, ‘бр.’ и т.н.");
                return;
            }


           // string selectedCurrency = "";
            if (currencyComboBox.SelectedItem != null)
            {
                selectedCurrency = currencyComboBox.SelectedItem.ToString();
            }
            else
            {
                // Обработка на случая, когато няма избран елемент
                MessageBox.Show("Моля, изберете валута.");
                return;
            }


            // Изчисления
            safetyStock = Math.Round((maxDailyUsage * maxDeliveryTime) - (avgDailyUsage * avgDeliveryTime)); // Предпазен запас
          carryingCost = Math.Round((allInventoryExpenses / totalInventoryCost) * 100); // Разходи за поддържане на запасите
          reorderPoint = Math.Round((leadTime * demand) + safetyStock); // Точка за повторна поръчка
          economicOrderQuantity = Math.Round(Math.Sqrt((2 * annualDemand * orderCost) / holdingCost)); // Икономично количество за поръчка

          safetyStockLabel.Text = "Предпазния запас трябва да бъде " + safetyStock + " " + unittextBox.Text;
          carryingCostLabel.Text = "Разходи за поддържане на запасите са " + carryingCost + "% от общата стойност на запасите";
          reorderPointLabel.Text = "Точка за повторна поръчка е " + reorderPoint + " " + unittextBox.Text;
          eoqLabel.Text = "Икономично количество за поръчка е " + economicOrderQuantity + " " + unittextBox.Text;

          buttoncalculate.Visible = false;
          buttonRestart.Visible = true;
          buttonExit.Visible = true;
          buttonSaveData.Visible = true;
          buttonDownloadData.Visible = false;

          foreach(TextBox texBox in textBoxesKey)
          {
              texBox.Enabled = false;
          }

            currencyComboBox.Enabled = false;

        }


        private void buttonRestart_Click(object sender, EventArgs e)
        {
           
            foreach (TextBox textBox in textBoxesKey)
            {
                textBox.Clear();
            }

            foreach(Label label in labels)
            {
                label.Text = " ";
            }

            currencyComboBox.Text = " ";

            buttoncalculate.Visible = true;
            buttonRestart.Visible = false;
            buttonExit.Visible = true;
            buttonSaveData.Visible = false;
            buttonDownloadData.Visible = false;

            buttonSaveData.BackColor = Color.Transparent;

            foreach (TextBox texBox in textBoxesKey)
            {
                texBox.Enabled = true;
            }

            currencyComboBox.Enabled = true;
        }

        private void buttonSaveData_Click(object sender, EventArgs e)
        {
            Item newItem = new Item();
            newItem.Name = nametextBox.Text;
            newItem.MaxDailyUsage = maxDailyUsage;
            newItem.MaxDeliveryTime = maxDeliveryTime;
            newItem.AvgDailyUsage = avgDailyUsage;
            newItem.AvgDeliveryTime = avgDeliveryTime;
            newItem.TotalInventoryCost = totalInventoryCost;
            newItem.AllInventoryExpenses = allInventoryExpenses;
            newItem.AnnualDemand = annualDemand;
            newItem.OrderCost = orderCost;
            newItem.HoldingCost = holdingCost;
            newItem.SafetyStock = safetyStock;
            newItem.CarryingCost = carryingCost;
            newItem.ReorderPoint = reorderPoint;
            newItem.EconomicOrderQuantity = economicOrderQuantity;


            // Добавяне на новия артикул към списъка
            items.Add(newItem);
            buttonDownloadData.Visible = true;
            buttonSaveData.BackColor = Color.LightGreen;
        }

        private void buttonDownloadData_Click(object sender, EventArgs e)
        {

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (Item item in items)
                    {
                        writer.WriteLine("********** Име на артикула: " + item.Name + " **********");
                        writer.WriteLine(" ");
                        writer.WriteLine("********** Входни данни:" + " **********");
                        writer.WriteLine("Максимална дневна употреба: " + item.MaxDailyUsage + " " + unittextBox.Text);
                        writer.WriteLine("Максимално време за доставка: " + item.MaxDeliveryTime + " дни");
                        writer.WriteLine("Средна дневна употреба: " + item.AvgDailyUsage + " " + unittextBox.Text);
                        writer.WriteLine("Средно време за доставка: " + item.AvgDeliveryTime + " дни");
                        writer.WriteLine("Обща годишна стойност на запасите: " + item.TotalInventoryCost + " " + selectedCurrency);
                        writer.WriteLine("Всички разходи свързани със поддържането на запасите за една година: " + item.AllInventoryExpenses + " " + selectedCurrency);
                        writer.WriteLine("Годишно търсене: " + item.AnnualDemand + " " + unittextBox.Text);
                        writer.WriteLine("Разходи за поръчка: " + item.OrderCost + " " + selectedCurrency);
                        writer.WriteLine("Разходи за поддържане на запас (разходи за единица): " + item.HoldingCost + " " + selectedCurrency);
                        writer.WriteLine(" ");
                        writer.WriteLine("********** Резултати от изчисленията:" + " **********");
                        writer.WriteLine("Предпазния запас трябва да бъде: " + item.SafetyStock + " " + unittextBox.Text);
                        writer.WriteLine("Разходи за поддържане на запасите са: " + item.CarryingCost + "% от общата стойност на запасите");
                        writer.WriteLine("Точка за повторна поръчка е : " + item.ReorderPoint + " " + unittextBox.Text);
                        writer.WriteLine("Икономично количество за поръчка е : " + item.EconomicOrderQuantity + " " + unittextBox.Text);
                        writer.WriteLine(" ");
                    }
                }
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
