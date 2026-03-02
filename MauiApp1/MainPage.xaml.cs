using System.Collections.ObjectModel;
// ...existing code...
namespace MauiApp1;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		// Test Azure SQL connection and fetch records on startup
		TestAzureSqlConnection();
	}

	public ObservableCollection<DogRecord> Dogs { get; set; } = new();

	private async void TestAzureSqlConnection()
	{
		string connectionString = Secrets.ConnectionString;
		try
		{
			using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
			{
				await connection.OpenAsync();
				var command = new Microsoft.Data.SqlClient.SqlCommand("SELECT TOP 20 ID, Dog FROM Dogs", connection);
				using (var reader = await command.ExecuteReaderAsync())
				{
					Dogs.Clear();
					while (await reader.ReadAsync())
					{
						Dogs.Add(new DogRecord
						{
							ID = reader.GetInt32(0),
							Dog = reader.GetString(1)
						});
					}
				}
			}
		}
		catch (Exception ex)
		{
			await Application.Current.MainPage.DisplayAlert("Azure SQL Error", ex.Message, "OK");
		}
		DogsCollectionView.ItemsSource = Dogs;
	}

	public class DogRecord
	{
		public int ID { get; set; }
		public string Dog { get; set; }
	}
	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

		TestAzureSqlConnection();
	}
}

