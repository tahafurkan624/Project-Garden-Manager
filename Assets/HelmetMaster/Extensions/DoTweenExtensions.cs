using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace HelmetMaster.Extensions
{
	public static class DoTweenExtension
	{
		/// <summary>
		/// Tweens a BlendShape weight to the given value from current weight
		/// </summary>
		/// <param name="index">Index of the BlendShape in the SkinnedMeshRenderer</param>
		/// <param name="target">Value of the weight will be tweened to. Between 0-100</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>The tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOBlendShape(this SkinnedMeshRenderer renderer, int index, float target, float duration)
		{
			float value = renderer.GetBlendShapeWeight(index);
			return DOTween.To(() => value, x =>
			{
				value = x;
				renderer.SetBlendShapeWeight(index, value);
			}, target, duration).SetTarget(renderer);
		}

		/// <summary>
		/// Tweens a BlendShape weight to the given value from a given value
		/// </summary>
		/// <param name="index">Index of the BlendShape in the SkinnedMeshRenderer</param>
		/// <param name="from">Value of the weight will be tweened from. Between 0-100</param>
		/// <param name="target">Value of the weight will be tweened to. Between 0-100</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>The tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOBlendShape(this SkinnedMeshRenderer renderer, int index, float from, float target, float duration)
		{
			renderer.SetBlendShapeWeight(index, from);
			float value = from;
			return DOTween.To(() => value, x =>
			{
				value = x;
				renderer.SetBlendShapeWeight(index, value);
			}, target, duration).SetTarget(renderer);
		}

		public static TweenerCore<float, float, FloatOptions> DOAlpha(this Graphic graphic, float alpha, float duration)
		{
			float a = graphic.color.a;
			return DOTween.To(() => a, x =>
			{
				Color color = graphic.color;
				color.a = a = x;
				graphic.color = color;
			}, alpha, duration).SetTarget(graphic);
		}

		public static Sequence DOFancyJump(this Transform transform, Vector3 endValue, float jumpPower, float duration, float bumpH, float stretchH)
		{
			Vector3 originalScale = transform.localScale;
			Vector3 originalRotation = transform.localEulerAngles;
			Sequence sequence = DOTween.Sequence();

			float scaleDuration = 0.3f;
			float rotateDuration = 0.3f;

			float _b = Mathf.Sqrt(1 / bumpH);
			float _s = Mathf.Sqrt(1 / stretchH);

			Vector3 jumpDir = (endValue - transform.position).normalized;
			Vector3 rotationAxis = Vector3.Cross(transform.up, jumpDir);

			sequence.Join(transform.DOScale(new Vector3(_b, bumpH, _b), scaleDuration).SetEase(Ease.Linear));
			sequence.Append(transform.DOScale(new Vector3(_s, stretchH, _s), scaleDuration).SetEase(Ease.Linear));
			sequence.Join(transform.DORotate(rotationAxis * 15, rotateDuration).SetEase(Ease.InCubic));
			sequence.Insert(scaleDuration * 2, transform.DOJump(endValue, jumpPower, 1, duration).SetEase(Ease.Linear));

			sequence.Insert(scaleDuration * 2 + duration / 4, transform.DOScale(originalScale, scaleDuration).SetEase(Ease.Linear));
			sequence.Insert(scaleDuration * 2 + duration / 4, transform.DORotate(rotationAxis * -15, duration / 2).SetEase(Ease.Linear));

			sequence.Insert(scaleDuration * 2 + duration / 2, transform.DOScale(new Vector3(_s, stretchH, _s), scaleDuration).SetEase(Ease.Linear));

			sequence.Insert(scaleDuration * 2 + duration, transform.DOScale(new Vector3(_b, bumpH, _b), scaleDuration).SetEase(Ease.Linear));
			sequence.Insert(scaleDuration * 2 + duration, transform.DORotate(originalRotation, rotateDuration).SetEase(Ease.OutCubic));

			sequence.Append(transform.DOScale(originalScale, scaleDuration).SetEase(Ease.Linear));

			sequence.SetTarget(transform);
			return sequence;
		}

		#region Camera Offset

		/// <summary>
		/// Tweens the Cinemachine Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineTransposer vCam, Vector3 endValue, float duration)
		{
			Vector3 value = vCam.m_FollowOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_FollowOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine Framing Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineFramingTransposer vCam, Vector3 endValue, float duration)
		{
			Vector3 value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine Orbital Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineOrbitalTransposer vCam, Vector3 endValue, float duration)
		{
			Vector3 value = vCam.m_FollowOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_FollowOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine aim Composer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraAimOffSet(this CinemachineComposer vCam, Vector3 endValue, float duration)
		{
			Vector3 value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine aim Group Composer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraAimOffSet(this CinemachineGroupComposer vCam, Vector3 endValue, float duration)
		{
			Vector3 value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		#endregion
	}
}