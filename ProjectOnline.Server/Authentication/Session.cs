namespace ProjectOnline.Server.Authentication
{
	using SecureRemotePassword;
	using System;

	public class Session
	{
		public readonly Account Account;

		private SrpServer? server;
		private SrpEphemeral? ephemeral;

		public string? EncryptionKey { get; private set; }

		public Session(Account account)
		{
			this.Account = account;
		}

		public string PrepareLogin()
		{
			this.server = new SrpServer();
			this.ephemeral = this.server.GenerateEphemeral(this.Account.Verifier);

			return this.ephemeral.Public;
		}

		public string? Login(string ephemeral, string proof)
		{
			if (this.server == null || this.ephemeral == null)
				return null;

			try
			{
				var session = this.server.DeriveSession(
					this.ephemeral.Secret,
					ephemeral,
					this.Account.Salt,
					this.Account.Username,
					this.Account.Verifier,
					proof
				);

				this.EncryptionKey = session.Key;

				return session.Proof;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
