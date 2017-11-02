﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextAI
{
	public eAIStateType StateType;
	public BaseObject TargetObject;
	public Vector3 Position;
}

public class BaseAI : BaseObject
{
    protected List<NextAI> ListNextAI = new List<NextAI>();
	protected eAIStateType _CurrentAIState = eAIStateType.AI_STATE_IDLE;
	public eAIStateType CurrentAIState
	{ get { return _CurrentAIState; } }

	bool bUpdateAI = false;
	bool bAttack = false;
	public bool IsAttack
	{
		get { return bAttack; }
		set { bAttack = value; }
	}

	bool bEnd = false;
	public bool END
	{
		get { return bEnd; }
		set { bEnd = value; }
	}

	protected Vector3 MovePosition = Vector3.zero;
	Vector3 PreMovePosition = Vector3.zero;

	Animator Anim = null;
	NavMeshAgent NavAgent = null;

	public Animator ANIMATOR
	{
		get
		{
			if(Anim == null)
			{
				Anim = SelfObject.GetComponentInChildren<Animator>();
			}
			return Anim;
		}
	}

	public NavMeshAgent NAV_MESH_AGENT
	{
		get
		{
			if(NavAgent == null)
			{
				NavAgent = SelfObject.GetComponent<NavMeshAgent>();
			}
			return NavAgent;
		}
	}

	void ChangeAnimation()
	{

		if(ANIMATOR == null)
		{
			Debug.LogError(SelfObject.name +
				" 에게 Animator 가 없습니다. ");
			return;
		}

		ANIMATOR.SetInteger("State", (int)CurrentAIState);
	}

	protected bool MoveCheck()
	{
		if(NAV_MESH_AGENT.pathStatus == NavMeshPathStatus.PathComplete)
		{
			if(NAV_MESH_AGENT.hasPath == false || 
				NAV_MESH_AGENT.pathPending == false)
			{
				return true;
			}
		}

		return false;
	}

	protected void SetMove(Vector3 position)
	{
		if (PreMovePosition == position)
			return;

		PreMovePosition = position;
		NAV_MESH_AGENT.isStopped = false;
		NAV_MESH_AGENT.SetDestination(position);
	}

	protected void Stop()
	{
		MovePosition = Vector3.zero;
		NAV_MESH_AGENT.isStopped = true;
	}


	public void JoyMove(Vector3 position)
	{
		bUpdateAI = false;
		ClearAI();
		SetMove(position);
		ProcessMove();
	}


	protected virtual void ProcessIdle()
	{
		_CurrentAIState = eAIStateType.AI_STATE_IDLE;
		ChangeAnimation();
	}

	protected virtual void ProcessMove()
	{
		_CurrentAIState = eAIStateType.AI_STATE_MOVE;
		ChangeAnimation();
	}

	protected virtual void ProcessAttack()
	{
        TargetComponent.ThrowEvent(ConstValue.EventKey_SelectSkill, 0);

		_CurrentAIState = eAIStateType.AI_STATE_ATTACK;
		ChangeAnimation();
	}

	protected virtual void ProcessDie()
	{
		_CurrentAIState = eAIStateType.AI_STATE_DIE;
		ChangeAnimation();
	}

    // 이곳에는 base가 코루틴으로 돌아가기 때문에 각 행동에서의 base 상태를 지정해줄 수 있다.
    // normalAI override한 함수에서는 예외적으로 패턴을 넣어 줄수 있다.
	protected virtual IEnumerator Idle()
	{
        bUpdateAI = false;
		yield break;
	}

    
	protected virtual IEnumerator Move()
	{
		bUpdateAI = false;
		yield break;
	}

	protected virtual IEnumerator Attack()
	{
        
		bUpdateAI = false;
		yield break;
	}

	protected virtual IEnumerator Die()
	{
		bUpdateAI = false;
		yield break;
	}


	public virtual void AddNextAI(
		eAIStateType nextStateType,
		BaseObject targetObject = null,
		Vector3 position = new Vector3())
	{
		NextAI nextAI = new NextAI
		{
			StateType = nextStateType,
			TargetObject = targetObject,
			Position = position
		};

		ListNextAI.Add(nextAI);
	}

	void SetNextAI(NextAI nextAI)
	{
		if (nextAI.TargetObject != null)
		{
			TargetComponent.ThrowEvent(
				ConstValue.ActorData_SetTarget,
				nextAI.TargetObject);
		}

		if(nextAI.Position != Vector3.zero)
		{
			MovePosition = nextAI.Position;
		}

		switch (nextAI.StateType)
		{
			case eAIStateType.AI_STATE_IDLE:
				ProcessIdle();
				break;
			case eAIStateType.AI_STATE_ATTACK:
				{
					if(nextAI.TargetObject !=null)
					{
						SelfTransform.forward =
							(nextAI.TargetObject.SelfTransform.position
							- SelfTransform.position).normalized;
					}

					ProcessAttack();
				}
				break;
			case eAIStateType.AI_STATE_MOVE:
				ProcessMove();
				break;
			case eAIStateType.AI_STATE_DIE:
				ProcessDie();
				break;
		}
	}

	public void UpdateAI()
	{
        if (END == true)
            return;

		if (bUpdateAI == true)
			return;

		if(ListNextAI.Count > 0)
		{
            //애니메이션 상태 변화
			SetNextAI(ListNextAI[0]);

            ListNextAI.RemoveAt(0);
		}
        
        // 죽었을때
		if(ObjectState == eBaseObjectState.STATE_DIE)
		{
			ListNextAI.Clear();
			ProcessDie();
		}

		bUpdateAI = true;

        //현재 상태에 따른 코루틴 실행
		switch (CurrentAIState)
		{
			case eAIStateType.AI_STATE_IDLE:
				StartCoroutine("Idle");
				break;
			case eAIStateType.AI_STATE_ATTACK:
				StartCoroutine("Attack");
				break;
			case eAIStateType.AI_STATE_MOVE:
				StartCoroutine("Move");
				break;
			case eAIStateType.AI_STATE_DIE:
				StartCoroutine("Die");
				break;
		}

	}

	public void ClearAI()
	{
		ListNextAI.Clear();
	}

	public void ClearAI(eAIStateType stateType)
	{
		// #3 Lamda
		// () => {}
		ListNextAI.RemoveAll(
			
			(nextAI) =>
			{
				return nextAI.StateType == stateType;
			}

			// (nextAI) => nextAI.StateType == stateType;

			);
	}

}
