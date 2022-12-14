using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
	[Header("Snake Movement")]
	[SerializeField]
	private	float			moveTime = 0.15f;                    // 한 칸 이동 시간
	private	Vector2			moveDirection = Vector2.right;		// 이동 방향
	// 실제 Snake 이동방향은 안바뀌었지만 입력 방향에 의해 같은 축으로 이동이 가능한 것을 방지
	private	Vector2			lastInputDirection = Vector2.right;
	public static bool move = true; //움직이게 할지 말지

	[Header("Snake Segments")]
	[SerializeField]
	public	Transform		segmentPrefab;						// Segment 프리팹 //
	[SerializeField]
	public int				spawnSegmentCountAtStart = 2;       // 게임 시작 시 Snake의 길이 (머리 포함)
	public List<Transform>	segments = new List<Transform>();   // Snake와 Segment를 관리하는 리스트 //
	Transform segment;
	public static bool p_SegmentDel;

	[Header("MapCollider")]
	[SerializeField]
	private	BoxCollider2D	mapCollider2D;                      // 맵을 벗어나는지 검사하기 위한 맵의 충돌 범위 정보

	public GameObject Fog;
	public GameObject gameOverSc; //게임 오버 화면
	public static int Score;
	public static int MaxScore;
	public static int changeText; //UI_Text.cs 에서 얻은 아이템에 따라 텍스트가 변경됨
	int maxBoomLength;

	private IEnumerator Start()
	{
		Setup();
		maxBoomLength = spawnSegmentCountAtStart-1;
		changeText = 0;
		Score = 0;
		while (move)
		{
			MoveSegments();

			yield return StartCoroutine("WaitForSeconds", moveTime);
		}
	}

	private void Update()
	{
		if(MaxScore < Score)
        {
			MaxScore = Score;
        }

		if (Input.GetKeyUp(KeyCode.D))
		{
			Debug.Log("위험");
			segmentPrefab.tag = "Segment";
		}

		if (Input.GetKeyUp(KeyCode.A))
		{
			Debug.Log("ㅇㅇ");
			segments.RemoveRange(1, maxBoomLength);
			p_SegmentDel = true;
			StartCoroutine(BoolFalse());
			maxBoomLength = 0;
		}

		if (Input.GetKeyUp(KeyCode.S))
		{
			Debug.Log("안개안개 은신술");
			Fog.SetActive(true);
			StartCoroutine(FogOff());
		}
		
		// 현재 x축으로 이동중이면 y축 방향으로만 방향 전환 가능
		if ( moveDirection.x != 0 )
		{
			if ( Input.GetKeyDown(KeyCode.UpArrow) )			lastInputDirection = Vector2.up;
			else if ( Input.GetKeyDown(KeyCode.DownArrow) )		lastInputDirection = Vector2.down;
		}
		// 현재 y축으로 이동중이면 x축 방향으로만 방향 전환 가능
		else if ( moveDirection.y != 0 )
		{
			if ( Input.GetKeyDown(KeyCode.LeftArrow) )			lastInputDirection = Vector2.left;
			else if ( Input.GetKeyDown(KeyCode.RightArrow) )	lastInputDirection = Vector2.right;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ( collision.CompareTag("Item") )
		{
			Score++;
			int ran = Random.Range(0, 20);

			if (ran <= 9)
			{
				changeText = 1;
				Debug.Log("꼬리증가");
				AddSegment();
				maxBoomLength++;
			}

            if (ran >= 10 && ran <= 16)
            {
				changeText = 2;
				Debug.Log("추가점수");
                Score++;
            }

            if (ran >= 17 && ran <= 18)
            {
				changeText = 3;
				Debug.Log("안개안개 은신술");
				Fog.SetActive(true);
				StartCoroutine(FogOff());

			}

			if (ran == 19)
			{
				changeText = 4;
				Debug.Log("꼬리 초기화!!!!!");
				segments.RemoveRange(1, maxBoomLength);
				p_SegmentDel = true;
				StartCoroutine(BoolFalse());
				maxBoomLength = 0;
			}
		}

		if ( collision.CompareTag("Segment") )
		{
			GameOver();
			
		}
	}

	private IEnumerator WaitForSeconds(float time)
	{
		float current = 0;
		float percent = 0;

		while ( percent < 1 )
		{
			current += Time.deltaTime;
			percent = current / time;

			yield return null;
		}
	}

	private void Setup()
	{
		// Snake 본체를 segments 리스트에 저장
		segments.Add(transform);

		// Snake를 쫓아다니는 꼬리(segment 오브젝트)를 생성하고, segments 리스트에 저장
		for ( int i = 0; i < spawnSegmentCountAtStart-1; ++ i )
		{
			AddSegment();
		}
	}

	private void AddSegment()
	{
		segment = Instantiate(segmentPrefab);
		segment.position = segments[segments.Count - 1].position;
		segments.Add(segment);
	}

	private void MoveSegments()
	{
		// 실제 이동할 때 마지막 입력 방향(lastInputDirection)으로 이동하도록 설정
		moveDirection = lastInputDirection;

		for ( int i = segments.Count - 1; i > 0; -- i )
		{
			segments[i].position = segments[i-1].position;
		}

		transform.position = (Vector2)transform.position + moveDirection;

		Bounds bounds = mapCollider2D.bounds;

		if ( transform.position.x < bounds.min.x || transform.position.x > bounds.max.x ||
			 transform.position.y < bounds.min.y || transform.position.y > bounds.max.y )
		{
			GameOver();
			//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void GameOver()
    {
		move = false;
		gameOverSc.SetActive(true);
	}

	IEnumerator BoolFalse()
	{
		yield return new WaitForSeconds(0.01f);
		p_SegmentDel = false;
	}

	IEnumerator FogOff()
    {
		yield return new WaitForSeconds(3f);
		Fog.SetActive(false);
	}

}

