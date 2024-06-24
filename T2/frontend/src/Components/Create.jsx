import React from "react";

export default function Create() {
  let questionIndex = 2;
  const addquestion = () => {
    const question = document.createElement("div");
    question.className = "question";
    question.innerHTML = `
            <p>Question ${questionIndex}</p>
      <input type="text" placeholder="Question" id=${
        "Q" + questionIndex
      } class="questionfield"/>
            <p class="option_heading">Options:</p>
            <div class="options">
            <input type="text" id=${
              "Q" + questionIndex + "_Option1"
            } placeholder="Option 1" class="option" />
            <input type="text" id=${
              "Q" + questionIndex + "_Option2"
            } placeholder="Option 2" class="option" />
            <input type="text" id=${
              "Q" + questionIndex + "_Option3"
            } placeholder="Option 3" class="option" />
            <input type="text" id=${
              "Q" + questionIndex + "_Option4"
            } placeholder="Option 4" class="option" />
            </div>
            <div className="answer">
            <p class="ans_header">Choose the correct answer:</p>
            <select name="answer" id=${
              "Q" + questionIndex + "_answer"
            } class="answer_selector" >
              <option value="Option1">Option1</option>
              <option value="Option2">Option2</option>
              <option value="Option3">Option3</option>
              <option value="Option4">Option4</option>
            </select>
            </div>
            <br />
            <hr />
    `;
    document.getElementById("questions").appendChild(question);
    questionIndex++;
  };
  const create = () => {
    let title = document.getElementById("title").value;
    let questions = [];
    let question, option1, option2, option3, option4, answer;
    for (let i = 1; i < questionIndex; i++) {
      question = document.getElementById("Q" + i).value;
      option1 = document.getElementById("Q" + i + "_Option1").value;
      option2 = document.getElementById("Q" + i + "_Option2").value;
      option3 = document.getElementById("Q" + i + "_Option3").value;
      option4 = document.getElementById("Q" + i + "_Option4").value;
      answer = document.getElementById("Q" + i + "_answer").value;
      let questionObj = {
        question,
        option1,
        option2,
        option3,
        option4,
        answer,
      };
      questions.push(questionObj);
    }
    let datatosend = { title, questions };
    console.log(datatosend);
  };
  return (
    <div>
      <form className="create-quiz">
        <input type="text" placeholder="TITLE" id="title" />
        <br />
        <hr />
        <div id="questions">
          <div className="question">
            <p>Question 1</p>
            <input
              type="text"
              placeholder="Question"
              id="Q1"
              className="questionfield"
            />
            <p className="option_heading">Options:</p>
            <div className="options">
              <input
                type="text"
                id="Q1_Option1"
                placeholder="Option 1"
                className="option"
              />
              <input
                type="text"
                id="Q1_Option2"
                placeholder="Option 2"
                className="option"
              />
              <input
                type="text"
                id="Q1_Option3"
                placeholder="Option 3"
                className="option"
              />
              <input
                type="text"
                id="Q1_Option4"
                placeholder="Option 4"
                className="option"
              />
            </div>
            <div className="answer">
              <p className="ans_header">Choose the correct answer:</p>
              <select name="answer" id="Q1_answer" className="answer_selector">
                <option value="Option1">Option1</option>
                <option value="Option2">Option2</option>
                <option value="Option3">Option3</option>
                <option value="Option4">Option4</option>
              </select>
            </div>
            <br />
            <hr />
          </div>
        </div>
        <button type="button" id="add-question" onClick={() => addquestion()}>
          Add Question
        </button>
        <button
          type="button"
          onClick={() => {
            create();
          }}
          id="create-button"
        >
          Create
        </button>
      </form>
    </div>
  );
}
