using System.Collections.Generic;
using System.Linq;
using SampleExam;
using SampleExam.Features.Question;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Question
{
    public class EditTests : IntegrationTestBase, IClassFixture<MapperFixture>
    {
        private readonly MapperFixture mapper;

        public EditTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory,
            MapperFixture mapper
        ) : base(factory, dbContextFactory)
        {
            this.mapper = mapper;
        }

        [Fact]
        public async void ShouldEditTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var questionItems = await httpCallHelper.CreateQuestion();
            var questionItemsPublished = await httpCallHelper.CreateQuestion(loggedUser: questionItems.Item1);
            var questionItems2 = await httpCallHelper.CreateQuestion();
            var question1 = questionItems.Item5;
            var question2 = questionItems2.Item5;
            var questionPublished = questionItemsPublished.Item5;
            var examPublished = questionItemsPublished.Item3;
            var user1 = questionItems.Item1;
            var radioQuestionData = TestData.Question.Edit.NewQuestionData();
            var checkboxQuestionData = TestData.Question.Edit.NewQuestionData(false);

            var link1 = $"/questions/{question1.Id}";
            var link2 = $"/questions/{question2.Id}";
            var linkPublished = $"/questions/{questionPublished.Id}";

            await client.PutUnauthorized(link1, new Edit.Request() { Question = radioQuestionData });

            client.Authorize(user1.Token);

            var tempQuestion = TestData.Question.Edit.NewQuestionDataWithEmptyText();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithTooLongText();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithFewerAnswerOption();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithMoreAnswerOption();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataRadioWithTwoRight();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataCheckboxWithSingleRight();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithAllRight();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithAllWrong();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithAllRight(false);
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithAllWrong(false);
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithEmptyAnswerText();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });


            tempQuestion = TestData.Question.Edit.NewQuestionDataWithTooLongAnswerText();
            await client.PutBadRequest(link1, new Edit.Request() { Question = tempQuestion });

            //other user's exam 
            await client.PutNotFound(link2, new Edit.Request() { Question = radioQuestionData });

            //edit published exam question
            await client.PutSucessfully(linkPublished, new Edit.Request() { Question = radioQuestionData });
            await httpCallHelper.PublishExam(examPublished.Id);
            await client.PutNotFound(linkPublished, new Edit.Request() { Question = radioQuestionData });

            //sucess
            //null text and null answer
            var questionData = TestData.Question.Edit.NewQuestionDataWithNullTextAndAnswer();
            var resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertEqual(question1, resultDto);

            //null text and same answers
            questionData = mapper.Mapper.Map<Edit.QuestionData>(resultDto);
            questionData.Text = null;
            resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertEqual(question1, resultDto);

            //same text and null answers
            questionData = mapper.Mapper.Map<Edit.QuestionData>(resultDto);
            questionData.Answers = null;
            resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertEqual(question1, resultDto);

            //with new answer
            questionData = mapper.Mapper.Map<Edit.QuestionData>(resultDto);
            var count = questionData.Answers.Count();
            var answers = new List<Edit.AnswerData>(questionData.Answers);
            var newAnserOptionData = TestData.Question.Edit.GenerateAnswerOption(count);
            answers.Add(newAnserOptionData);
            questionData.Answers = answers;
            resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertNotEqual(question1, resultDto);
            Assert.True(question1.UpdatedAt < resultDto.UpdatedAt);
            Assert.True(resultDto.AnswerOptions.Count() == count + 1);
            var answerOptions = resultDto.AnswerOptions.OrderByDescending(e => e.Id).ToArray();
            var answerOptionAdded = answerOptions[0];
            AssertHelper.AssertEqual(answerOptionAdded, newAnserOptionData);

            //only added anser changed.
            for (int i = 1; i < answerOptions.Length; i++)
            {
                var ao = answerOptions[i];
                Assert.True(ao.UpdatedAt < answerOptionAdded.UpdatedAt);
            }
            question1 = resultDto;

            //with new text
            questionData = mapper.Mapper.Map<Edit.QuestionData>(resultDto);
            questionData.Text = questionData.Text + "_NewOne";
            resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertNotEqual(question1, resultDto);
            Assert.True(question1.Text != resultDto.Text);
            Assert.True(question1.UpdatedAt < resultDto.UpdatedAt);
            foreach (var ao in resultDto.AnswerOptions)
            {
                Assert.True(ao.UpdatedAt < resultDto.UpdatedAt);
            }
            question1 = resultDto;
            //with new answer and deleted and edited
            questionData = mapper.Mapper.Map<Edit.QuestionData>(resultDto);
            count = questionData.Answers.Count();
            answers = new List<Edit.AnswerData>(questionData.Answers);
            newAnserOptionData = TestData.Question.Edit.GenerateAnswerOption(count);
            var answerEdited = answers.First();
            answerEdited.Text = answerEdited.Text + "_Edited";
            var answerDeleted = answers.Where(e => e.Id != answerEdited.Id && !e.IsRight).Last();
            var answerUnchanged = answers.Where(e => e.Id != answerEdited.Id && e.Id != answerDeleted.Id && !e.IsRight).Last();
            answerUnchanged.Text = null;
            answers.Add(newAnserOptionData);
            answers.Remove(answerDeleted);
            questionData.Answers = answers;
            resultDto = await client.PutQuestionSucessfully(link1, new Edit.Request() { Question = questionData });
            AssertHelper.AssertNotEqual(question1, resultDto);
            Assert.True(question1.UpdatedAt < resultDto.UpdatedAt);
            Assert.True(resultDto.AnswerOptions.Count() == count);
            answerOptions = resultDto.AnswerOptions.OrderByDescending(e => e.Id).ToArray();
            answerOptionAdded = answerOptions[0];
            var answerOptionEdited = answerOptions.Where(e => e.Id == answerEdited.Id).First();
            var answerOptionUnchanged = answerOptions.Where(e => e.Id == answerUnchanged.Id).First();
            AssertHelper.AssertEqual(answerOptionAdded, newAnserOptionData);
            AssertHelper.AssertEqual(answerOptionEdited, answerEdited);
            Assert.True(resultDto.UpdatedAt == answerOptionEdited.UpdatedAt);
            Assert.True(answerOptionAdded.UpdatedAt == answerOptionEdited.UpdatedAt);
            Assert.True(answerOptionUnchanged.Text.Length > 1);
            Assert.True(answerOptionUnchanged.UpdatedAt < answerOptionEdited.UpdatedAt);
            foreach (var ao in resultDto.AnswerOptions)
            {
                Assert.True(ao.Id != answerDeleted.Id);
                if (ao.Id != answerOptionAdded.Id && ao.Id != answerOptionEdited.Id)
                {
                    Assert.True(ao.UpdatedAt < answerOptionEdited.UpdatedAt);
                }
            }
            question1 = resultDto;

            // some ok cases
            tempQuestion = TestData.Question.Edit.NewQuestionDataWithNullAnswerOption();
            await client.PutSucessfully(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithNullText();
            await client.PutSucessfully(link1, new Edit.Request() { Question = tempQuestion });

            tempQuestion = TestData.Question.Edit.NewQuestionDataWithEmptyAnswerOption();
            await client.PutSucessfully(link1, new Edit.Request() { Question = tempQuestion });


        }

    }
}