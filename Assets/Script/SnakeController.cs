using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
	[Header("Snake Movement")]
	[SerializeField]
	private	float			moveTime = 0.15f;                    // �� ĭ �̵� �ð�
	private	Vector2			moveDirection = Vector2.right;		// �̵� ����
	// ���� Snake �̵������� �ȹٲ������ �Է� ���⿡ ���� ���� ������ �̵��� ������ ���� ����
	private	Vector2			lastInputDirection = Vector2.right;
	public static bool move = true; //�����̰� ���� ����

	[Header("Snake Segments")]
	[SerializeField]
	private	Transform		segmentPrefab;						// Segment ������
	[SerializeField]
	public int				spawnSegmentCountAtStart = 6;		// ���� ���� �� Snake�� ���� (�Ӹ� ����)
	private	List<Transform>	segments = new List<Transform>();	// Snake�� Segment�� �����ϴ� ����Ʈ

	[Header("MapCollider")]
	[SerializeField]
	private	BoxCollider2D	mapCollider2D;                      // ���� ������� �˻��ϱ� ���� ���� �浹 ���� ����

	public GameObject gameOverSc; //���� ���� ȭ��
	public static int Score;

	private IEnumerator Start()
	{
		Setup();

		while (move)
		{
			MoveSegments();

			yield return StartCoroutine("WaitForSeconds", moveTime);
		}
	}

	private void Update()
	{
		// ���� x������ �̵����̸� y�� �������θ� ���� ��ȯ ����
		if ( moveDirection.x != 0 )
		{
			if ( Input.GetKeyDown(KeyCode.UpArrow) )			lastInputDirection = Vector2.up;
			else if ( Input.GetKeyDown(KeyCode.DownArrow) )		lastInputDirection = Vector2.down;
		}
		// ���� y������ �̵����̸� x�� �������θ� ���� ��ȯ ����
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
			Debug.Log(Score);
			AddSegment();
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
		// Snake ��ü�� segments ����Ʈ�� ����
		segments.Add(transform);

		// Snake�� �Ѿƴٴϴ� ����(segment ������Ʈ)�� �����ϰ�, segments ����Ʈ�� ����
		for ( int i = 0; i < spawnSegmentCountAtStart-1; ++ i )
		{
			AddSegment();
		}
	}

	private void AddSegment()
	{
		Transform segment = Instantiate(segmentPrefab);
		segment.position = segments[segments.Count-1].position;
		segments.Add(segment);
	}

	private void MoveSegments()
	{
		// ���� �̵��� �� ������ �Է� ����(lastInputDirection)���� �̵��ϵ��� ����
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
}

