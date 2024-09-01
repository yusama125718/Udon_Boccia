
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Boccia_ScoreBoard : UdonSharpBehaviour
{
    [Header("Red Score")]
    [SerializeField] private TextMeshProUGUI red_score_text = null;
    [Header("Red Round Score")]
    [Header("※Max Roundより多く要素を入れること")]
    [SerializeField] private TextMeshProUGUI[] red_round_score_text = null;
    [Header("Blue Score")]
    [SerializeField] private TextMeshProUGUI blue_score_text = null;
    [Header("Blue Round Score")]
    [Header("※Max Roundより多く要素を入れること")]
    [SerializeField] private TextMeshProUGUI[] blue_round_score_text = null;
    [Header("Max Round")]
    [SerializeField] private int max_round = 6;

    void start(){
        Reset_Board();
    }

    // スコアボード更新
    public void Update_Round(int red_score, int blue_score, int red_diff, int blue_diff, int round){
        red_score_text.text = red_score.ToString();
        blue_score_text.text = blue_score.ToString();

        // maxラウンドより多かったらラウンドスコアは更新しない
        if (round > max_round) return;

        red_round_score_text[round - 1].text = red_diff.ToString();
        blue_round_score_text[round - 1].text = blue_diff.ToString();
    }

    // スコアボードリセット
    public void Reset_Board(){
        red_score_text.text = "0";
        blue_score_text.text = "0";
        foreach(TextMeshProUGUI mesh in red_round_score_text) mesh.text = "";
        foreach(TextMeshProUGUI mesh in blue_round_score_text) mesh.text = "";
    }
}
