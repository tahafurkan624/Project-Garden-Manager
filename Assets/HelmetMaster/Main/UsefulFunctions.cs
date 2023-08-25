using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using HelmetMaster.Extensions;
using UnityEngine;

namespace HelmetMaster.Main
{
	public static class UsefulFunctions
	{
		private static Camera mainCamera;
		public static Camera MainCamera
		{
			get
			{
				if (mainCamera != null) return mainCamera;

				mainCamera = Camera.main;
				return mainCamera;
			}
		}

		public static Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;
			return (uu * p0) + (2 * u * t * p1) + (tt * p2);
		}

		public static void ObliqueLaunch(Rigidbody rb, Vector3 targetPos, float maxHeight)
		{
			float g = Physics.gravity.y;
			float displacementY = targetPos.y - rb.transform.position.y;
			Vector3 displacementXZ =
				new Vector3(targetPos.x - rb.transform.position.x, 0, targetPos.z - rb.transform.position.z);
			float time = Mathf.Sqrt(-2 * maxHeight / g) + Mathf.Sqrt(2 * (displacementY - maxHeight) / g);
			Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * g * maxHeight);
			Vector3 velocityXZ = displacementXZ / time;
			rb.velocity = velocityY + velocityXZ;
		}

		public static void DrawObliquePath(Transform transform, Vector3 targetPos, float maxHeight, LineRenderer lineRenderer, int resolution = 10)
		{
			float g = Physics.gravity.y;
			float displacementY = targetPos.y - transform.position.y;
			Vector3 displacementXZ =
				new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);
			float time = Mathf.Sqrt(-2 * maxHeight / g) + Mathf.Sqrt(2 * (displacementY - maxHeight) / g);
			Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * g * maxHeight);
			Vector3 velocityXZ = displacementXZ / time;

			Vector3 initialVelocity = velocityY + velocityXZ;

			lineRenderer.positionCount = resolution + 1;

			for (int i = 0; i <= resolution; i++)
			{
				float simTime = i / (float)resolution * time;
				Vector3 displacement = (initialVelocity * simTime) + simTime * simTime * Physics.gravity / 2f;
				Vector3 drawPoint = transform.position + displacement;
				lineRenderer.SetPosition(i, drawPoint);
			}
		}

		public static float Map(float value, float min1, float max1, float min2, float max2)
		{
			return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
		}

		public static string GetMonthName(int month)
		{
			switch (month)
			{
				default:
				case 0: return "January";
				case 1: return "February";
				case 2: return "March";
				case 3: return "April";
				case 4: return "May";
				case 5: return "June";
				case 6: return "July";
				case 7: return "August";
				case 8: return "September";
				case 9: return "October";
				case 10: return "November";
				case 11: return "December";
			}
		}

		//Returns the first 3 letter of the month
		public static string GetMonthNameShort(int month)
		{
			return GetMonthName(month).Substring(0, 3);
		}

		// Return a color going from Red to Yellow to Green, like a heat map
		public static Color GetRedGreenColor(float value)
		{
			float r = 0f;
			float g = 0f;
			if (value <= .5f)
			{
				r = 1f;
				g = value * 2f;
			}
			else
			{
				g = 1f;
				r = 1f - (value - .5f) * 2f;
			}

			return new Color(r, g, 0f, 1f);
		}

		//Returns a percent value for a given float value between 0 and 1
		public static string GetPercentString(float f, bool includeSign = true)
		{
			return Mathf.RoundToInt(f * 100f) + (includeSign ? "%" : "");
		}

		//Returns the position where a position falls on the UI(2d)
		public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera = null)
		{
			worldCamera = worldCamera != null ? worldCamera : Camera.main;
			Ray ray = worldCamera.ScreenPointToRay(screenPosition);
			Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
			float distance;
			xy.Raycast(ray, out distance);
			return ray.GetPoint(distance);
		}

		public static Vector3 GetDirToMouseWorld(Vector3 fromPosition, Camera camera = null)
		{
			camera = camera == null ? Camera.main : camera;
			Vector3 mouseWorldPosition = GetMouseWorldPosition3D(camera);
			return (mouseWorldPosition - fromPosition).normalized;
		}

		//Remember to use it on the object with a collider!
		public static Vector3 GetMouseWorldPosition3D(Camera camera)
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
			{
				return raycastHit.point;
			}
			else
			{
				return Vector3.zero;
			}
		}

		//Circle positions
		public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount)
		{
			List<Vector3> ret = new List<Vector3>();
			for (int i = 0; i < positionCount; i++)
			{
				int angle = i * (360 / positionCount);
				Vector3 dir = VectorExtensions.ApplyRotationToVector(new Vector3(0, 1), angle);
				Vector3 pos = position + dir * distance;
				ret.Add(pos);
			}

			return ret;
		}

		// Distance is constant between the circles
		public static List<Vector3> GetPositionListAround(Vector3 position, float ringDistance, int[] ringPositionCount)
		{
			List<Vector3> ret = new List<Vector3>();
			float initialRingDistance = ringDistance;
			for (int ring = 0; ring < ringPositionCount.Length; ring++)
			{
				List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance, ringPositionCount[ring]);
				ret.AddRange(ringPositionList);
				ringDistance += initialRingDistance;
			}

			return ret;
		}

		//More than 1 circle
		public static List<Vector3> GetPositionListAround(Vector3 position, float[] ringDistance, int[] ringPositionCount)
		{
			List<Vector3> ret = new List<Vector3>();
			for (int ring = 0; ring < ringPositionCount.Length; ring++)
			{
				List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance[ring], ringPositionCount[ring]);
				ret.AddRange(ringPositionList);
			}

			return ret;
		}


		//Direction of the circle is given
		public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount, Vector3 direction)
		{
			List<Vector3> ret = new List<Vector3>();
			for (int i = 0; i < positionCount; i++)
			{
				int angle = i * (360 / positionCount);
				Vector3 dir = VectorExtensions.ApplyRotationToVector(direction, angle);
				Vector3 pos = position + dir * distance;
				ret.Add(pos);
			}

			return ret;
		}

		public static List<Vector3> GetPositionListAlongDirection(Vector3 position, Vector3 direction, float distancePerPosition, int positionCount)
		{
			List<Vector3> ret = new List<Vector3>();
			for (int i = 0; i < positionCount; i++)
			{
				Vector3 pos = position + direction.normalized * (distancePerPosition * i);
				ret.Add(pos);
			}

			return ret;
		}

		//2 vektor arasindaki istenilen pozisyon sayisina gore pozisyonlari atlaya atlaya dondurur.  000 009 3 ise 003 006 009 gibi
		public static List<Vector3> GetPositionListAlongAxis(Vector3 positionStart, Vector3 positionEnd, int positionCount, bool includeStart = false)
		{
			Vector3 direction = (positionEnd - positionStart).normalized;
			if (!includeStart)
			{
				float distancePerPosition = (positionEnd - positionStart).magnitude / positionCount;
				return GetPositionListAlongDirection(positionStart + direction * (distancePerPosition), direction, distancePerPosition, positionCount);
			}
			else
			{
				float distancePerPosition = (positionEnd - positionStart).magnitude / (positionCount - 1);
				return GetPositionListAlongDirection(positionStart, direction, distancePerPosition, positionCount);
			}
		}

		public static Vector3 GetVectorFromAngle(int angle)
		{
			// angle = 0 -> 360
			float angleRad = angle * (Mathf.PI / 180f);
			return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
		}

		public static Vector3 GetVectorFromAngle(float angle)
		{
			// angle = 0 -> 360
			float angleRad = angle * (Mathf.PI / 180f);
			return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
		}

		public static bool IsPositionInsideCamera(Vector3 posTarget, Camera cam = null)
		{
			cam = cam == null ? Camera.main : cam;
			Vector3 viewport = cam.WorldToViewportPoint(posTarget);
			bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
			bool inFrontOfCamera = viewport.z > 0;

			return inCameraFrustum && inFrontOfCamera;
		}

		public static Vector2 WorldToCanvasPosition(Transform t, Canvas c)
		{
			RectTransform CanvasRect = c.GetComponent<RectTransform>();
			Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(t.position);
			Vector2 WorldObject_ScreenPosition = new Vector2(
				((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
				((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
			return WorldObject_ScreenPosition;
		}

		private static bool Is01(float a)
		{
			return a > 0 && a < 1;
		}

		public static T GetLowestFromList<T>(this List<T> list, Func<T, float> predicate)
		{
			if (list.Count <= 0) throw new Exception("List cannot be null");

			var lowest = predicate(list[0]);
			var lowestIdx = 0;
			for (var i = 0; i < list.Count; i++)
			{
				var value = predicate(list[i]);

				if (!(value < lowest)) continue;
				lowestIdx = i;
				lowest = value;
			}

			return list[lowestIdx];
		}

		public static T GetLowestFromArray<T>(this T[] array, Func<T, float> predicate)
		{
			if (array.Length <= 0) throw new Exception("List cannot be null");

			var lowest = predicate(array[0]);
			var lowestIdx = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var value = predicate(array[i]);

				if (!(value < lowest)) continue;
				lowestIdx = i;
				lowest = value;
			}

			return array[lowestIdx];
		}

		public static T GetHighestFromList<T>(this List<T> list, Func<T, float> predicate)
		{
			if (list.Count <= 0) throw new Exception("List cannot be null");

			var highest = predicate(list[0]);
			var highestIdx = 0;
			for (var i = 0; i < list.Count; i++)
			{
				var value = predicate(list[i]);

				if (!(value > highest)) continue;
				highestIdx = i;
				highest = value;
			}

			return list[highestIdx];
		}

		public static T GetHighestFromArray<T>(this T[] array, Func<T, float> predicate)
		{
			if (array.Length <= 0) throw new Exception("List cannot be null");

			var highest = predicate(array[0]);
			var highestIdx = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var value = predicate(array[i]);

				if (!(value > highest)) continue;
				highestIdx = i;
				highest = value;
			}

			return array[highestIdx];
		}

		public static int GetRep(int num, int divided)
		{
			if(divided == 0) throw new Exception("Divided cannot be zero");

			var rep = 0;
			while (num > divided)
			{
				num -= divided;
				rep++;
			}

			return rep;
		}

		// Use case: this.Invoke(() => Function(0, false), 1f);
		public static void Invoke(this MonoBehaviour mb, Action f, float delay)
		{
			mb.StartCoroutine(InvokeRoutine(f, delay));
		}
 
		private static IEnumerator InvokeRoutine(Action f, float delay)
		{
			yield return new WaitForSeconds(delay);
			f();
		}

		public static Path CreateBezierPath(Vector3[] positions, int subdivisionsXSegment = 5)
		{
			var path = new Path(PathType.CatmullRom, positions, subdivisionsXSegment);
			return path;
		}

		public static Path CreateLinearPath(Vector3[] positions)
		{
			var path = new Path(PathType.Linear, positions, 1);
			return path;
		}
		
		public static bool IsEven(int num)
		{
			return num % 2 == 0;
		}
	}
}