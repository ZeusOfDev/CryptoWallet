using RestSharp;

public static class PriceHelper
{
    private static readonly RestClient _client = new RestClient("https://api.coingecko.com/api/v3");

    public static async Task<decimal> GetPriceAsync(string coinId, string vsCurrency = "usd")
    {
        var request = new RestRequest("simple/price", Method.Get);
        request.AddParameter("ids", coinId);
        request.AddParameter("vs_currencies", vsCurrency);

        var response = await _client.ExecuteAsync<Dictionary<string, Dictionary<string, decimal>>>(request);

        if (!response.IsSuccessful || response.Data == null)
        {
            throw new Exception($"Failed to fetch price for {coinId}");
        }

        return response.Data[coinId][vsCurrency];
    }
}
