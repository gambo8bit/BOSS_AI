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
        SetMove(Target.transform.position); //목적지 설정

        //거리가 1 이하면
        if(Vector3.Distance(Target.transform.position, transform.position) < 0.1f)
        {
            Stop();
            AddNextAI(eAIStateType.AI_STATE_ATTACK);
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
        return base.Attack();
    }

    protected override IEnumerator Die()
    {
        return base.Die();
    }
    int rot_count = 0;
   void Skill_1()
    {
        rot_count++;
        transform.Rotate(0, 1, 0);
        
        if(rot_count >= 720)
        {
            rot_count = 0;
            b_Skill_1_ready = false;
        }
        
    }
    private void Update()
    {

        if(!b_Skill_1_ready)
        time += Time.deltaTime;

        if (time > 10)
        {
            time = 0;
            b_Skill_1_ready = true;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        Target = other.transform;
    }
}
