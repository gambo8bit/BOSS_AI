using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BOSS_2_SKILL : BaseAI
{
    bool b_Skill_1_ready = false;
    float time = 0;
    Collider boss_collider;
    Transform Target = null;
    

    private void Awake()
    {
        boss_collider = GetComponent<BoxCollider>();
    }

    protected override IEnumerator Idle()
    {
       if( Target!= null)
        {

            AddNextAI(eAIStateType.AI_STATE_MOVE);

        }



        return base.Idle();
    }

    protected override IEnumerator Move()
    {

        //거리가 1 이하면
        if(Vector3.Distance(Target.transform.position, transform.position) < Target.transform.localScale.z)
        {
            Stop();
            AddNextAI(eAIStateType.AI_STATE_ATTACK);
        }
        else
        {
        SetMove(Target.transform.position); //목적지 설정
            transform.LookAt(Target);
            AddNextAI(eAIStateType.AI_STATE_MOVE);
        }
        
        
        return base.Move();
    }

    protected override IEnumerator Attack()
    {
        if(b_Skill_1_ready)
        {
            Skill_1();


           
        }
        else
        {


        }
        AddNextAI(eAIStateType.AI_STATE_IDLE);
        return base.Attack();
    }

    protected override IEnumerator Die()
    {
        return base.Die();
    }
    

    void Skill_1()
    {


        NAV_MESH_AGENT.enabled= false;
        transform.position += Vector3.up * 10;




        b_Skill_1_ready = false;
        ////transform.Rotate(90, 0, 0);
        //rot_count++;
        //transform.Rotate(0, 1, 0);

        //if (rot_count >= 720)
        //{
        //    rot_count = 0;
        //}

    }
    private void Update()
    {

        if(!b_Skill_1_ready)
        time += Time.deltaTime;

        if (time > 5f)
        {
            time = 0;
            b_Skill_1_ready = true;
        }
            
        UpdateAI();
    }

    private void OnTriggerEnter(Collider other)
    {
        Target = other.transform;
    }
}
