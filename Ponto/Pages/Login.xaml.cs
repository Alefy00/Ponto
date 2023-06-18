using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ponto.Security;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Ponto.Pages
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            string matricula = txtMatricula.Text;
            string senha = txtSenha.Text;

            bool isValid = await Authenticate(matricula, senha);

            if (isValid)
            {
                string token = JwtManager.GenerateToken(matricula);

                await Navigation.PushAsync(new Home(token));
            }
            else
            {
                await DisplayAlert("Erro", "Credenciais inválidas", "OK");
            }
        }

        private async Task<bool> Authenticate(string matricula, string senha)
        {
            string apiUrl = "https://ida.ceub.br/api/v1.0/pp/Credencial";
            var httpClient = new HttpClient();

            // Adicionar a chave do Basic Auth
            string username = "PP.CEUB.BR";
            string password = "JoBonizm9uId/uM1ED3Q7WKtr9umltxBUl+UepBHF2A=";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

            var requestData = new
            {
                drt = matricula,
                senha
            };

            var dataJson = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl, dataJson);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Verificar se a resposta contém a palavra "sucesso" no conteúdo
                if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("sucesso"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
