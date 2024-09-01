using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;
using System.Collections.Generic;

// 同期モードを手動に固定
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Udon_Boccia : UdonSharpBehaviour
{
    [Header("Red Balls")]
    [SerializeField] private GameObject[] red_balls = null;
    [Header("Blue Balls")]
    [SerializeField] private GameObject[] blue_balls = null;
    [Header("Jack Ball(White Ball)")]
    [SerializeField] private GameObject jack_ball = null;
    [Header("Max Round")]
    [SerializeField] private int max_round = 6;
    [Header("Score Board")]
    [SerializeField] private GameObject score_board = null;

    [UdonSynced]private int red_score;
    [UdonSynced]private int blue_score;
    [UdonSynced]private int round;
    [UdonSynced]private int last_red_score;
    [UdonSynced]private int last_blue_score;

    // 得点判定
    public void Round_End(){
        // 最大ラウンド以上だった場合動かない
        if(round >= max_round) return;
        // オーナーでなければオーナーにする
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Vector3 jack_position = jack_ball.transform.position;

        // 赤青それぞれのボールとジャックボールの距離を短い順にリストに格納
        // 小数第3位(1cm)で切り捨て
        float[] red_range = new float[6];
        float[] blue_range = new float[6];

        for (int i = 0; i < red_balls.Length; i++){
            float distance = Vector3.Distance(jack_position, red_balls[i].transform.position);
            distance = distance * 100;
            distance = (float) Math.Floor(distance) / 100;
            red_range[i] = distance;
        }
        Sort(red_range);

        for (int i = 0; i < blue_balls.Length; i++){
            float distance = Vector3.Distance(jack_position, blue_balls[i].transform.position);
            distance = distance * 100;
            distance = (float) Math.Floor(distance) / 100;
            blue_range[i] = distance;
        }
        Sort(blue_range);

        // 得点計算
        last_red_score = 0;
        last_blue_score = 0;

        if(red_range[0] == blue_range[0]){
            // 距離が同じな場合
            last_red_score++;
            last_blue_score++;
            for (int i = 1; i < red_range.Length; i++){
                if(red_range[i - 1] == red_range[i]) last_red_score++;
                else break;
            }
            for (int i = 1; i < blue_range.Length; i++){
                if(blue_range[i - 1] == blue_range[i]) last_blue_score++;
                else break;
            }
        }
        else if(red_range[0] < blue_range[0]){
            // 赤が一番近い場合
            last_red_score++;
            for (int i = 1; i < red_range.Length; i++){
                if(red_range[i] < blue_range[0]) last_red_score++;
                else break;
            }
        }
        else if(red_range[0] > blue_range[0]){
            // 青が一番近い場合
            last_blue_score++;
            for (int i = 1; i < blue_range.Length; i++){
                if(blue_range[i] < red_range[0]) last_blue_score++;
                else break;
            }
        }

        // スコア加算
        red_score += last_red_score;
        blue_score += last_blue_score;
        round++;

        // 同期
        RequestSerialization();
        score_board.GetComponent<Boccia_ScoreBoard>().Update_Round(red_score, blue_score, last_red_score, last_blue_score, round);
    }

    // ゲームリセット
    public void Reset_Game(){
        // オーナーでなければオーナーにする
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

        // 変数をリセット
        red_score = 0;
        blue_score = 0;
        last_red_score = 0;
        last_blue_score = 0;
        round = 0;

        // 同期
        RequestSerialization();
        score_board.GetComponent<Boccia_ScoreBoard>().Reset_Board();
    }

    // 同期完了時にスコアボード更新
    void OnDeserialization(){
        if (round == 0) score_board.GetComponent<Boccia_ScoreBoard>().Reset_Board();
        else score_board.GetComponent<Boccia_ScoreBoard>().Update_Round(red_score, blue_score, last_red_score, last_blue_score, round);
    }

    // 昇順にソート
    float[] Sort(float[] array){
        for (int i = 1; i < array.Length; i++){
            for (int j = i; j > 0; j--){
                if (array[j] < array[j - 1]){
                    float cache = array[j - 1];
                    array[j - 1] = array[j];
                    array[j] = cache;
                }
                else break;
            }
        }
        return array;
    }
}
