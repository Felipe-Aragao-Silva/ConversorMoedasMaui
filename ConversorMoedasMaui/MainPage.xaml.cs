using System.Globalization;

namespace ConversorMoedasMaui
{
    public partial class MainPage : ContentPage
    {

        private readonly Dictionary<string, decimal> _toBRL = new()
        {
            // Esse dictionary ele faz com que  os valores de conversão sejam fixos e fictícios para o exemplo funcionar
            //e tambem ele da os valores de cada moeda em relação ao BRL
            {"BRL",1.00m }, // 1 BRL= 1BRL
            {"USD", 5.60m }, // 1USD = 5,60 BRL
            {"EUR", 6.10m } // 1 EUR = 6,10 BRL
        };

        private readonly Dictionary<string, string> _cultureCurrency = new()
        {

            // esse dictionary ele faz com que o app saiba qual cultura usar para formatar a moeda
            //essa cultura é usada para formatar o valor de acordo com a moeda selecionada
            {"BRL", "pt-BR" },
            {"USD", "en- US" },
            {"EUR", "de - DE" }
        };

        public MainPage()

        {
            // esse metodo ele inicializa os componentes da pagina e chama o metodo InitDefaults que seta os valores iniciais dos pickers e labels
            InitializeComponent();

            InitDefaults();
        }

        void InitDefaults()
        {
            // esse metodo seta os valores iniciais dos pickers e labels
            FromPicker.SelectedIndex = IndexOf(FromPicker, "BRL");

            ToPicker.SelectedIndex = IndexOf(ToPicker, "USD");

            InfoLabel.Text = "Valores fictícios";
            ResultLabel.Text = string.Empty;
        }

        //isso serve paradescobrir a posiçao do item no picker
        //tipo indece de um livro 
        int IndexOf(Picker picker, string item) => picker.Items.IndexOf(item);

        void OnInverterClicked(object sendet, EventArgs e)
        {
            //esse metodo inverte os valores dos pickers quando o botao é clicado
            var fromIndex = FromPicker.SelectedIndex;
            FromPicker.SelectedIndex = ToPicker.SelectedIndex;
            ToPicker.SelectedIndex = fromIndex;
            //InfoRateHint();
        }

        void OnPickerChanged(object sender, EventArgs e)
        {
            //Limpa o resultado quando o usuário muda a seleção de moeda
            // InfoRateHint();
            ResultLabel.Text = string.Empty;

        }
        void OnAmountChanged(object sender, TextChangedEventArgs e)
        {
            //Amount é o valor que você quer converter, por exemplo, 100 reais para dólares
            //Limpa o resultado quando o usuário muda o valor do Entry
            if (String.IsNullOrWhiteSpace
                (AmountEntry.Text))
            {
                InfoLabel.Text = "Digite um valor válido";

            } else
            {
                //InfoRateHint();
            }
        }
        void InfoRateHint()
        {
            var from = GetFrom();
            var to = GetTo();

            if (from is null || to is null) return;

            if (from == to)
            {
                InfoLabel.Text = "Mesma moeda selecionada";
                return;
            }
            else
            {
                var rate = Rate(from, to);
                InfoLabel.Text = $"1 {from} = {rate: 0.####} {to}";
            }
        } // trará a mensagem educativa
           
        async void OnConvertClicked(object sender, EventArgs e)
        {
            try
            {
                var from = GetFrom();
                var to = GetTo();

                if (string.IsNullOrWhiteSpace(AmountEntry.Text))
                {
                    await DisplayAlert("Atenção", "Digite um valor válido", "OK");
                    return;
                }

                if (!decimal.TryParse(AmountEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) || amount < 0)
                {
                    await DisplayAlert("Atenção", "Valor Inválido", "OK");
                    return;
                }

                var result = Convert(from, to, amount);

                var culture = new CultureInfo(_cultureCurrency[to]);

                var formatted = result.ToString("C", culture);

                ResultLabel.Text = $"{amount} {from} = {formatted}";

                // 100BRL = XXXXX USD
                InfoRateHint();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao converter", "OK");

            }

        }

        decimal Convert(string from, string to, decimal amount)
        {
            // esse metodo faz a conversão de uma moeda para outra
            if (from == to) return amount;
            // Converte o valor digitado para BRL (Real), multiplicando pelo fator de conversão da moeda de origem
            var brl = amount * _toBRL[from];
            // Converte o valor em BRL para a moeda de destino, dividindo pelo fator de conversão da moeda de destino
            var result = brl / _toBRL[to];
            return decimal.Round(result, 4);
        }
        decimal Rate(string from, string to)
        {
            // Se as moedas forem iguais, retorna 1 (não há conversão)
            if (from == to) return 1.00m;
            // Converte 1 unidade da moeda de origem para BRL
            var brl = 1.00m * _toBRL[from];
            // Divide o valor em BRL pelo fator de conversão da moeda de destino,
            // obtendo assim quanto vale 1 unidade da moeda de origem na moeda de destino
            var toValue = brl / _toBRL[to];
            return decimal.Round(toValue, 6);
        }


        // ? => forçar o sistema a autualizar o valor mesmo que seja nulo 
        string? GetFrom() => FromPicker.SelectedIndex >= 0 ? FromPicker.Items[FromPicker.SelectedIndex] : null;

        string? GetTo() => ToPicker.SelectedIndex >= 0 ? ToPicker.Items[ToPicker.SelectedIndex] : null;

    }
}