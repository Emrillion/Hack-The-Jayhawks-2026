public class Question
{
	public string Text;
	public string A, B, C, D;
	public int CorrectAnswer; // 1=A, 2=B, 3=C, 4=D

	public Question(string text, string a, string b, string c, string d, int correct)
	{
		Text = text;
		A = a; B = b; C = c; D = d;
		CorrectAnswer = correct;
	}
}
