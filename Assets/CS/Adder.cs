using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Adder : MonoBehaviour
{
    public bool isAllowComputeInput;                            // 수식 입력 가능 여부
    public bool isNonNumInput;                                  // 숫자 입력 차단

    public string inputStr = string.Empty;                      // 현재 입력값
    public List<string> inputValues = new List<string>();       // 입력값 + 수식 보관

    string tempValue = string.Empty;                            // 임시로 복사한 값

    public TextMeshProUGUI recodeText;                          // 계산식 텍스트
    public TextMeshProUGUI resultText;                          // 계산 결과 텍스트

    public GameObject insertObject;                             // 고정지출 삽입 버튼 오브젝트
    public TMP_InputField feTitleInputfield;                    // 고정지출 삽입 타이틀 입력 인풋필드

    // =================================================================

    private readonly string[] allowNumList = {"0","1","2","3","4","5","6","7","8","9"};
    private readonly string[] allowComList = {"+","-","*","/"};

    void Start()
    {
        InputClear();
    }

    void Update()
    {
        // 고정지출에 값 삽입 버튼 표시 여부
        // 길이가 1일 경우 (계산된 값만 있는 경우)에 값 삽입이 가능하도록 함
        // insertObject.SetActive(inputValues.Count == 1);
    }

    // =========================================================

    public void Compute() // 계산
    {
        if (inputValues.Count <= 1) {
            resultText.text = "Error : 계산할 결과가 없습니다.";
            return;
        } else if(inputValues.Count % 2 == 0 && inputStr.Equals(string.Empty)) {
            resultText.text = "Error : 수식 뒤에는 값이 있어야 합니다";
            return;
        } else {
            inputValues.Add(inputStr);
            ComputeFirst();     // 곱하기, 나누기 먼저 계산
            ComputeSecond();    // 덧셈, 뺄셈 계산

            InitParam();        // 계산 후 변수 초기화
            insertObject.SetActive(true);
        }
    }

    void ComputeFirst() // 곱하기, 나누기 먼저 계산
    {
        // 홀수는 수식
        for(int i = 1; i < inputValues.Count; i+=2) {
            if(inputValues[i].Equals("*") || inputValues[i].Equals("/")) {
                float temp = 0;

                if(inputValues[i].Equals("*")) {
                    temp = float.Parse(inputValues[i-1]) * float.Parse(inputValues[i+1]);
                } else {
                    temp = float.Parse(inputValues[i-1]) / float.Parse(inputValues[i+1]);
                }

                inputValues.RemoveAt(i-1);
                inputValues.RemoveAt(i-1);
                inputValues.RemoveAt(i-1);

                inputValues.Insert(i-1,temp.ToString());

                // 뒤에도 비슷한 경우가 있을 수 있기에 재귀 호출로 체크함
                ComputeFirst();
            }
        }
    }

    void ComputeSecond() // 덧셈, 뺄셈 계산
    {
        // 홀수는 수식
        for(int i = 1; i < inputValues.Count; i+=2) {
            if(inputValues[i].Equals("+") || inputValues[i].Equals("-")) {
                float temp = 0;

                if(inputValues[i].Equals("+")) {
                    temp = float.Parse(inputValues[i-1]) + float.Parse(inputValues[i+1]);
                } else {
                    temp = float.Parse(inputValues[i-1]) - float.Parse(inputValues[i+1]);
                }

                inputValues.RemoveAt(i-1);
                inputValues.RemoveAt(i-1);
                inputValues.RemoveAt(i-1);

                inputValues.Insert(i-1,temp.ToString());

                // 뒤에도 비슷한 경우가 있을 수 있기에 재귀 호출로 체크함
                ComputeSecond();
            }
        }
    }

    // =========================================================

    public void InputNumber(string input)// 숫자, 기호 입력
    {
        // 값 : 1   3   5   7   .....
        // 식 :   2   4   6   8   .....

        insertObject.SetActive(false);

        // 수식 입력만 처리함
        foreach(var allow in allowComList) {
            if(input.Equals(allow)) {

                if(isAllowComputeInput) {
                    if(inputStr != string.Empty)
                        UpdateInputStr();       // 입력값이 있을 경우 입력값을 입력 리스트에 넣고 입력값 초기화
                } else {
                    inputValues.RemoveAt(inputValues.Count - 1);    // 리스트에 추가된 수식을 제거
                }
                
                inputStr += input;              // 인풋값을 전체 임시 입력값에 넣고 
                UpdateInputStr();               // 수식 입력값을 입력 리스트에 넣고 입력값 초기화
                ListPrint();                    // UI 업데이트

                isAllowComputeInput = false;    // 다음 형식에 수식이 올 수 없도록 함
                isNonNumInput       = false;
                return;
            }
        }

        if(isNonNumInput) return;

        // 숫자 입력만 처리함
            // 허용된 숫자 형식이 아닌 경우 예외처리함
        foreach(var allow in allowNumList) {
            if(input.Equals(allow)) {

                // 인풋값을 전체 임시 입력값에 넣고 UI 업데이트
                inputStr += input;
                ListPrint();

                // 다음 형식이 입력될 수 있도록 턴 넘김
                isAllowComputeInput = true;
                return;
            }
        }
    }

    // =========================================================

    public void InputMySalary() // 내 월급
    {
        ListPrint();
    }

    public void InputFixedExpense() // 고정 지출
    {
        ListPrint();
    }

    public void InputClear() // 전부 삭제
    {
        isAllowComputeInput = false;
        isNonNumInput       = false;

        inputValues = new List<string>();

        recodeText.text = string.Empty;
        resultText.text = string.Empty;

        inputStr = string.Empty;
    }

    public void InputDelete() // 지우기
    {
        if(inputStr == string.Empty) return;

        string temp = string.Empty;
        for(int i = 0; i < inputStr.Length - 1; i++) {
            temp += inputStr[i];
        }

        inputStr = temp;
        ListPrint();
    }

    private void InitParam() // 계산 후 변수 초기화
    {
            isAllowComputeInput = true;
            isNonNumInput       = true;

            resultText.text = inputValues[0];
            recodeText.text = string.Empty;
            inputStr = string.Empty;
    }

    public void InsertFE() // FE 타이틀 입력 후 고정지출 리스트에 값 삽입
    {
        GameManager.GM.InsertFEL( feTitleInputfield.text, int.Parse(inputValues[0].ToString()) );
        feTitleInputfield.text = string.Empty;
    }

    // =========================================================
   
    void ListPrint()  // 입력된 값을 UI에 업데이트하는 함수
    {
        string tempStr = "";

        foreach (var str in inputValues)
            tempStr += str;

        recodeText.text = tempStr + inputStr;     // GUI에 출력
    }

    void UpdateInputStr() // 현재 임지저장된 입력값을 리스트에 넣고 입력값 문자열 초기화
    {
        inputValues.Add(inputStr);
        inputStr = string.Empty;
    }
}
