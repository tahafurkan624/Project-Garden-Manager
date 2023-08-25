using _Main._Scripts._General;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public TruckArea TruckArea;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public bool IsActive = false, GoingReverse;
        float distanceTravelled;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if(!IsActive) return;

            if (pathCreator != null)
            {
                distanceTravelled += (GoingReverse ? -1 : 1) * (speed * Time.deltaTime);
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                // transform.rotation = GoingReverse ? pathCreator.path.GetRotationAtDistanceRev(distanceTravelled, endOfPathInstruction)
                //     : pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }

            if (distanceTravelled > pathCreator.path.length && !GoingReverse) IsActive = false;
            if (distanceTravelled < 0 && GoingReverse)
            {
                distanceTravelled = 0;
                IsActive = false;
                TruckArea.Returned();
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        public void FixDistanceTravelled()
        {
            distanceTravelled = pathCreator.path.length - 0.1f;
        }
    }
}