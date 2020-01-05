using System;
using System.Collections.Generic;
using System.Linq;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Utils;

namespace SampleExamIntegrationTests.Helpers.Data
{
    public static class TestData
    {
        public static class User
        {
            public static class Create
            {
                public static SampleExam.Features.User.Create.UserData NewUserData()
                {
                    var uniqueEmail = $"{Guid.NewGuid().ToGuidString()}@example.com";
                    var userData = new SampleExam.Features.User.Create.UserData()
                    {
                        Firstname = "Namig",
                        Lastname = "Hajiyev",
                        Middlename = "Zakir",
                        GenderId = 1,
                        Dob = new DateTime(1986, 04, 07),
                        Email = uniqueEmail,
                        Password = "2aEvJPCF",
                        ConfirmPassword = "2aEvJPCF"
                    };
                    return userData;
                }

            }
        }

        public static class Exam
        {
            public static class Create
            {
                public static SampleExam.Features.Exam.Create.ExamData NewExamData(
                     bool includeTags = true, bool isPrivate = false, string[] extraTags = null)
                {
                    var uniqueString = Guid.NewGuid().ToGuidString();
                    var random = Utils.NewRandom();
                    var examData = new SampleExam.Features.Exam.Create.ExamData()
                    {
                        Title = $"{uniqueString}_Title",

                        Description = $"{uniqueString}_Description",

                        TimeInMinutes = random.Next(30, 120),

                        PassPercentage = random.Next(50, 100),

                        IsPrivate = isPrivate,
                    };

                    if (includeTags)
                    {
                        var newTags = new List<string>() { $"{uniqueString}_tag1", $"{uniqueString}_tag2", $"{uniqueString}_tag3" };
                        if (extraTags != null)
                        {
                            newTags.AddRange(extraTags);
                        }
                        examData.Tags = newTags;
                    }
                    return examData;
                }
            }
        }

        public static class Question
        {
            public static class Create
            {
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionData(bool isRadio = true)
                {
                    int questionTypeId = isRadio ? SeedData.QuestionTypes.Radio.Id : SeedData.QuestionTypes.Checkbox.Id;
                    var questionData = new SampleExam.Features.Question.Create.QuestionData() { QuestionTypeId = questionTypeId, Text = "Some  question text ?" };
                    var answers = new List<SampleExam.Features.Question.Create.AnswerData>();
                    for (int i = 0; i < 5; i++)
                    {
                        answers.Add(GenerateAnswerOption(i));
                    }
                    answers[0].IsRight = true;
                    answers[1].IsRight = isRadio ? false : true;
                    questionData.Answers = answers;
                    return questionData;

                }

                public static SampleExam.Features.Question.Create.AnswerData GenerateAnswerOption(int i)
                {
                    return new SampleExam.Features.Question.Create.AnswerData()
                    {
                        Text = $"Answer option {i}"
                    };
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithEmptyText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = "";
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithNullText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithTooLongText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = new string('a', SampleExam.Common.Constants.QUESTION_TEXT_LEN + 1);
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithEmptyAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = new List<SampleExam.Features.Question.Create.AnswerData>();
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithNullAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithFewerAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = questionData.Answers.Take(SampleExam.Common.Constants.QUESTION_ANSWER_MIN_COUNT - 1);
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithMoreAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = new List<SampleExam.Features.Question.Create.AnswerData>();
                    var count = SampleExam.Common.Constants.QUESTION_ANSWER_MAX_COUNT + 1;
                    for (int i = 0; i < count; i++)
                    {
                        var answerOption = GenerateAnswerOption(i);
                        answers.Add(answerOption);
                    }
                    questionData.Answers = answers;
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataRadioWithTwoRight()
                {
                    var questionData = NewQuestionData(true);
                    var answers = questionData.Answers.ToArray();
                    answers[0].IsRight = true;
                    answers[1].IsRight = true;
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataCheckboxWithSingleRight()
                {
                    var questionData = NewQuestionData(false);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = false;
                    }
                    answers[0].IsRight = true;
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithAllRight(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = true;
                    }
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithAllWrong(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = false;
                    }
                    return questionData;
                }

                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithEmptyAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = "";
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithNullAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Create.QuestionData NewQuestionDataWithTooLongAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = new string('a', SampleExam.Common.Constants.ANSWEROPTION_TEXT_LEN + 1);
                    return questionData;
                }
            }
            public static class Edit
            {
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionData(bool isRadio = true)
                {
                    int questionTypeId = isRadio ? SeedData.QuestionTypes.Radio.Id : SeedData.QuestionTypes.Checkbox.Id;
                    var questionData = new SampleExam.Features.Question.Edit.QuestionData() { QuestionTypeId = questionTypeId, Text = "Some  question text ?" };
                    var answers = new List<SampleExam.Features.Question.Edit.AnswerData>();
                    for (int i = 0; i < 5; i++)
                    {
                        answers.Add(GenerateAnswerOption(i));
                    }
                    answers[0].IsRight = true;
                    answers[1].IsRight = isRadio ? false : true;
                    questionData.Answers = answers;
                    return questionData;

                }

                public static SampleExam.Features.Question.Edit.AnswerData GenerateAnswerOption(int i)
                {
                    return new SampleExam.Features.Question.Edit.AnswerData()
                    {
                        Text = $"Answer option {i}"
                    };
                }


                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithEmptyText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = "";
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithNullText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithTooLongText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Text = new string('a', SampleExam.Common.Constants.QUESTION_TEXT_LEN + 1);
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithEmptyAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = new List<SampleExam.Features.Question.Edit.AnswerData>();
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithNullAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithNullTextAndAnswer(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = null;
                    questionData.Text = null;
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithFewerAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    questionData.Answers = questionData.Answers.Take(SampleExam.Common.Constants.QUESTION_ANSWER_MIN_COUNT - 1);
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithMoreAnswerOption(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = new List<SampleExam.Features.Question.Edit.AnswerData>();
                    var count = SampleExam.Common.Constants.QUESTION_ANSWER_MAX_COUNT + 1;
                    for (int i = 0; i < count; i++)
                    {
                        var answerOption = GenerateAnswerOption(i);
                        answers.Add(answerOption);
                    }
                    questionData.Answers = answers;
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataRadioWithTwoRight()
                {
                    var questionData = NewQuestionData(true);
                    var answers = questionData.Answers.ToArray();
                    answers[0].IsRight = true;
                    answers[1].IsRight = true;
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataCheckboxWithSingleRight()
                {
                    var questionData = NewQuestionData(false);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = false;
                    }
                    answers[0].IsRight = true;
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithAllRight(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = true;
                    }
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithAllWrong(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    foreach (var answer in answers)
                    {
                        answer.IsRight = false;
                    }
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithEmptyAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = "";
                    return questionData;
                }
                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithNullAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = null;
                    return questionData;
                }

                public static SampleExam.Features.Question.Edit.QuestionData NewQuestionDataWithTooLongAnswerText(bool isRadio = true)
                {
                    var questionData = NewQuestionData(isRadio);
                    var answers = questionData.Answers.ToArray();
                    answers[0].Text = new string('a', SampleExam.Common.Constants.ANSWEROPTION_TEXT_LEN + 1);
                    return questionData;
                }


            }
        }
    }
}