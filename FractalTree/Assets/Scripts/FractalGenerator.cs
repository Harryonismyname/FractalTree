using System.Collections;
using UnityEngine;

public class FractalGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Material branchMat;
    [SerializeField] Material leafMat;
    //theta is the "bend angle" for each branching
    [Range(0, 180)]
    [SerializeField] float thetaInDeg;
    [SerializeField] int fiboLimit = 30;
    [SerializeField] int startingFiboStep = 15;
    [SerializeField] int fiboStepSize = 1;
    [SerializeField] int branchesPerLimb = 4;
    [SerializeField] int branchLengthLimit = 1;
    [SerializeField] float startingBranch = 75;
    [SerializeField] float leafStartLength = 10;
    [Range(.01f, 1f)]
    [SerializeField] float stepLenPercentChange = .75f;

    Vector3 RandomFibo(float f) => new (0, f * Random.Range(-1, 2), 0);
    IEnumerator Tree(float limbLen, Vector3 nextBranchPos, Vector3 nextBranchEulerAngles, int n)
    {
        static void LoopN(ref int n, int fiboLimit)
        {
            if (n > fiboLimit) n = 1;
            if (n <= 0) n = fiboLimit;
        }
        // Fibo for Y rotation
        static int Fibo(int len)
        {
            static int FiboRecursion(int a, int b, int counter, int len)
            {
                if (counter <= len)
                {
                    return FiboRecursion(b, a + b, counter+1, len);
                }
                return a;
            }
            return FiboRecursion(0, 1, 1, len);
        }
        static void SetColor(GameObject cylinder, Material mat1, Material mat2, float limbLen, float leafStartLength)
        {
            MeshRenderer renderer = cylinder.GetComponentInChildren<MeshRenderer>();
            Material mat;
            mat = mat1;
            if (limbLen < leafStartLength)
            {
                mat = mat2;
            }
            renderer.material = mat;
        }
        static void CreateBranch(GameObject BranchPrefab, out GameObject cylinder,Vector3 nextBranchPos, Vector3 nextBranchEulerAngles, int f, float limbLen)
        {
            // Build and set the limb values
            GameObject limb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            limb.transform.SetLocalPositionAndRotation(nextBranchPos, Quaternion.Euler(nextBranchEulerAngles));
            // Build and set the cylinder values
            cylinder = Instantiate(BranchPrefab);
            cylinder.transform.parent = limb.transform;
            cylinder.transform.SetLocalPositionAndRotation(new Vector3(0, limbLen, 0), Quaternion.identity);
            cylinder.transform.localScale = new Vector3(0.15f * limbLen, limbLen, 0.15f * limbLen);

        }
        if (limbLen > branchLengthLimit)
        {
            LoopN(ref n, fiboLimit);
            int f = Fibo(n);
            yield return new WaitForSeconds(1.5f);

            // Create branch
            CreateBranch(prefab, out GameObject cylinder, nextBranchPos, nextBranchEulerAngles, f, limbLen);

            // Change Color Based on Length
            SetColor(cylinder, branchMat, leafMat, limbLen, leafStartLength);

            nextBranchPos = cylinder.transform.position + (cylinder.transform.up * limbLen);
            // Center
            StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n + fiboStepSize));

            nextBranchEulerAngles += thetaInDeg * transform.right;
            for (int i = 0; i <= branchesPerLimb; i++)
            {
                nextBranchEulerAngles = Quaternion.AngleAxis(Fibo(f+i) * thetaInDeg, transform.up) * nextBranchEulerAngles;
                StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n + fiboStepSize));
            }

/*
            //here is the recursive magic
            StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n + fiboStepSize));
            //now you have to go back to where you were
            nextBranchEulerAngles += transform.right * (-2 * thetaInDeg);
            //this does the other side (also recursive)
            StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n - fiboStepSize));
            // Forward
            nextBranchEulerAngles += transform.forward * (thetaInDeg);
            StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n + fiboStepSize));

            //Backward
            nextBranchEulerAngles += transform.forward * (-2 * thetaInDeg);
            StartCoroutine(Tree(limbLen * stepLenPercentChange, nextBranchPos, nextBranchEulerAngles + RandomFibo(f), n - fiboStepSize));
*/
        }
    }



    private void Awake()
    {
        Vector3 startingDir = transform.right;
        StartCoroutine(Tree(startingBranch, transform.position, startingDir, startingFiboStep));
    }
}
