namespace ProjectOnline.Server.Authentication
{
	public class Account
	{
		public readonly string Username;
		public readonly string Salt;
		public readonly string Verifier;

		public Account(string username, string salt, string verifier)
		{
			this.Username = username;
			this.Salt = salt;
			this.Verifier = verifier;
		}
	}
}
