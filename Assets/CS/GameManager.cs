using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public static string mySalaryValue = string.Empty;                      // 내 세후 월급
    public List<FE_ListClass> myFixedExpenses = new List<FE_ListClass>();    // 내 고청지출 리스트

    [System.Serializable]
    public class FE_ListClass
    {
        public string titleName = "";
        public int fixedExpense = 0;

        /// <summary>
        /// title = 지출 사유 <br/>
        /// money = 지출 금액
        /// </summary>
        public FE_ListClass(string title, int money)
        {
            titleName = title;
            fixedExpense = money;

            GM.myFixedExpenses.Add(this);
        }
    }

    public void Awake() { GM = this; }

    // ======================================================

    /// <summary>
    /// title = 지출 사유 <br/>
    /// money = 지출 금액
    /// </summary>
    public void InsertFEL(string title, int money)
    {
        // 제목이 비었거나 없을 경우 예외로 처리함
        if(title.Trim().Equals(string.Empty) || title.Trim().Equals("")) return;

        // 숫자가 입력되지 않았을 경우
        if(money == 0) return;

        // 숫자가 0 이하일 경우 -1을 곱해서 양수로 만듦
        if(money < 0) money = money * (-1);

        new FE_ListClass(title,money);
    }
}
