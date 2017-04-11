using System;
[Serializable]
public class QuestionDAO
{
	public string question;
	public string subQuestion;
	public string previewUrl;
	public int answerId;
	public AnswerDAO[] answers;
}
