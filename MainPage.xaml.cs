using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;  // Para usar CultureInfo.InvariantCulture

namespace ConversaoMoedas
{
    public partial class MainPage : ContentPage
    {
        private const double TaxaEuroParaReal = 6.10;  // Exemplo de taxa fixa para Euro
        private HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        // Método para buscar a cotação de Dólar para Real na API
        private async Task<double> ObterCotacaoDolarAsync()
        {
            try
            {
                string url = "https://economia.awesomeapi.com.br/last/USD-BRL";
                var response = await _httpClient.GetStringAsync(url);

                // Parse da resposta JSON
                var json = JObject.Parse(response);
                var cotacaoDolar = json["USDBRL"]["bid"].ToString();

                // Usar CultureInfo.InvariantCulture para garantir que o ponto seja reconhecido como separador decimal
                return double.Parse(cotacaoDolar, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível obter a cotação: {ex.Message}", "OK");
                return 0;
            }
        }

        // Método para converter Dólar para Real usando a cotação da API
        private async void ConverterDolarParaReal(object sender, EventArgs e)
        {
            if (double.TryParse(ValorEntrada.Text, out double valor))
            {
                double taxaDolarParaReal = await ObterCotacaoDolarAsync();

                if (taxaDolarParaReal > 0)
                {
                    double resultado = valor * taxaDolarParaReal;
                    ResultadoConversao.Text = $"{valor} dólares = {resultado:F2} reais";
                }
                else
                {
                    ResultadoConversao.Text = "Erro ao obter a cotação do Dólar.";
                }
            }
            else
            {
                ResultadoConversao.Text = "Por favor, insira um valor válido.";
            }
        }

        // Método para converter Real para Dólar
        private async void ConverterRealParaDolar(object sender, EventArgs e)
        {
            if (double.TryParse(ValorEntrada.Text, out double valor))
            {
                double taxaDolarParaReal = await ObterCotacaoDolarAsync();

                if (taxaDolarParaReal > 0)
                {
                    double resultado = valor / taxaDolarParaReal;
                    ResultadoConversao.Text = $"{valor} reais = {resultado:F2} dólares";
                }
                else
                {
                    ResultadoConversao.Text = "Erro ao obter a cotação do Dólar.";
                }
            }
            else
            {
                ResultadoConversao.Text = "Por favor, insira um valor válido.";
            }
        }

        // Método para limpar os campos
        private void LimparCampos(object sender, EventArgs e)
        {
            ValorEntrada.Text = string.Empty;
            ResultadoConversao.Text = "Resultado da conversão aparecerá aqui";
        }
    }
}
