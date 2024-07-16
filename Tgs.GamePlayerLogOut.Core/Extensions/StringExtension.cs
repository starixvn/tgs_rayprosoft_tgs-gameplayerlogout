namespace Tgs.GamePlayerLogOut.Core.Extensions;

public static class StringExtension
{
	public static string ToGamePlayerLogOutSubject(this string str)
	=> $"gp.player.logout.{str.Replace("_", ".").ToLower()}";
}
