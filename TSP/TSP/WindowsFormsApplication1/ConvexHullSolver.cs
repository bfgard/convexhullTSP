using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

namespace _2_convex_hull
{
    class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;

        public ConvexHullSolver() { }

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
        }

        public void Refresh(Hull points)
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            System.Drawing.PointF[] pointList = points.getPoints();
            Pen pens = new Pen(Brushes.Aqua);
            // Iterate over the points in the hull and draw lines between them
            for (int i= 0; i < pointList.Length; ++i)
            {
                g.DrawLine(pens, pointList[i], pointList[(i + 1) % pointList.Length]);
            }
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }

        public int[] Solve(List<System.Drawing.PointF> pointList)
        {
            /* Solve
               Time complexity: worst case - O(n^2) average case - O(n*logn)  
               Space complexity: 
            */
            System.Drawing.PointF[] originalPoints = new System.Drawing.PointF[pointList.Count];
            pointList.CopyTo(originalPoints);
            // Sort the points according to their X value with the smallest X on the left
            pointList.Sort((x,y) => x.X.CompareTo(y.X));
            System.Drawing.PointF[] allPoints = pointList.ToArray();
            // First call to the divide and conquer algorithm that kicks everything off
            Hull solution = DivideAndConquer(allPoints);
            System.Drawing.PointF[] solutionPoints = solution.getPoints();
            int[] indices = new int[solutionPoints.Length];
            int j = 0;
            for(int i = 0; i < originalPoints.Length; i++)
            {

                if (solutionPoints.Contains(originalPoints[i]))
                {
                    indices[j] = i;
                    j++;
                }
            }
            return indices;
        }

        public Hull DivideAndConquer(System.Drawing.PointF[] pointList)
        {
            /* Divide and Conquer
               Time complexity: O(n*logn) log n calls of recombine - O(n)
               Space complexity: O()
            */
            int pSize = pointList.Length;
            // base case: Hull of size 1
            if (pSize == 1)
                return new Hull(pointList);
            // Split the array of points into two equally sized arrays (ceiling to the left and floor to right)
            System.Drawing.PointF[] leftArray = new System.Drawing.PointF[pSize - pSize / 2];
            System.Drawing.PointF[] rightArray = new System.Drawing.PointF[pSize / 2];
            Array.Copy(pointList, 0, leftArray, 0, pSize - pSize / 2);
            Array.Copy(pointList, pSize - pSize / 2, rightArray, 0, pSize / 2);
            // Recursively call the divide and conquer on the newly divided arrays
            Hull left = DivideAndConquer(leftArray);
            Hull right = DivideAndConquer(rightArray);
            // After the Hulls return from the recursion they are combined to create a larger Hull
            Hull combinedHull = recombineHull(left, right);
            return combinedHull;       
        }

        public Hull recombineHull(Hull left, Hull right)
        {
            /* Recombine Hull
               Time complexity: O(n) 
               Space complexity: 
            */
            // Grab the array of points from each Hull
            System.Drawing.PointF[] leftArray = left.getPoints();
            System.Drawing.PointF[] rightArray = right.getPoints();
            
            // If working with Hulls of size 1, simply combine the two arrays 
            if (left.getCount() == 1 & right.getCount() == 1) {
                System.Drawing.PointF[] combinedPoints = new System.Drawing.PointF[leftArray.Length + rightArray.Length];
                leftArray.CopyTo(combinedPoints, 0);
                rightArray.CopyTo(combinedPoints, leftArray.Length);
                Hull combinedHull = new Hull(combinedPoints);
                return combinedHull; 
            }

            // Else calculate the tangency points
            int lsize = leftArray.Length;
            int rsize = rightArray.Length;
            // Find the upper tangency points and return their indices
            int[] upperTangency = findUpper(left, right);
            // Find the lower tangency points
            int[] lowerTangency = findLower(left, right);
            // Combine these tangency points into one array
            int[] allTangency = new int[upperTangency.Length + lowerTangency.Length];
            upperTangency.CopyTo(allTangency, 0);
            lowerTangency.CopyTo(allTangency, upperTangency.Length); 
            // Using the tangency points and the old arrays form a new combined array
            Hull newHull = createNewHull(left, right, allTangency);

            return newHull;
        }

        public double getSlope(PointF left, PointF right)
        {
            /* Calculates slope
               Time complexity: O(1)
               Space complexity: O(1)
            */
            // Calculate the slope of two points (multiplied by -1 to correct for the inverted y value) 
            return - (left.Y - right.Y) / (left.X - right.X); 
        }

        public Hull createNewHull(Hull left, Hull right, int[] allTangency)
        {
            /* Create a new hull
               Time complexity: O(n)
               Space complexity: 
            */
            // Put the two hulls back together again using the tangency points and excluding inner points
            System.Drawing.PointF[] leftArray = left.getPoints();
            System.Drawing.PointF[] rightArray = right.getPoints();

            List<System.Drawing.PointF> dynamicList = new List<System.Drawing.PointF>();
            // Start by adding the leftmost point from the left array
            dynamicList.Add(leftArray[left.getLeftMost()]);
            left.setCurrentIndex(left.getLeftMost());
            // Add points in the left array starting at the leftmost point and going until the upper tangency point in the left array
            while(left.getCurrentIndex() != allTangency[0])
                dynamicList.Add(left.getNextClock());

            //Add points in the right array from the upper right tangency point to lower right tangency point (moving clockwise)
            right.setCurrentIndex(allTangency[1]);
            while (right.getCurrentIndex() != allTangency[3])
            {
                dynamicList.Add(rightArray[right.getCurrentIndex()]);
                right.getNextClock();
            }
            // Add the lower right tangency point
            dynamicList.Add(rightArray[right.getCurrentIndex()]);

            // Add points from the lower right tangency point to the leftmost point on the left hull
            left.setCurrentIndex(allTangency[2]); 
            while (left.getCurrentIndex() != left.getLeftMost())
            {
                dynamicList.Add(leftArray[left.getCurrentIndex()]);
                left.getNextClock();
            }
            // Return the newly formed hull
            System.Drawing.PointF[] newArray = dynamicList.ToArray();
            return new Hull(newArray);
        }

        public int[] findUpper(Hull left, Hull right)
        {
            /* Find upper tangency points
               Time complexity: O(n)
               Space complexity: 
            */
            // Find the two upper tangency points in the left and right hulls
            System.Drawing.PointF[] leftArray = left.getPoints();
            System.Drawing.PointF[] rightArray = right.getPoints();
            // Get the index of the rightmost point in the left hull and the leftmost point in the right hull
            int leftIndex = left.getRightMost();
            int rightIndex = right.getLeftMost();
            // Set the current point values to the leftmost and rightmost points
            System.Drawing.PointF leftCurrent = leftArray[leftIndex];
            System.Drawing.PointF rightCurrent = rightArray[rightIndex];
            // Set the index of the point being pointed to in the hull class (used for iterating)
            left.setCurrentIndex(leftIndex);
            right.setCurrentIndex(rightIndex);
            // Create points that will store the next point for each hull
            System.Drawing.PointF nextLeft;
            System.Drawing.PointF nextRight;
            // Calculate the slope of the two current points being pointed to (leftmost and rightmost)
            double currentSlope = getSlope(leftCurrent, rightCurrent);
            // Create flags to indicate when a tangency point has been found and when both points have been found
            bool foundTangency = false;
            bool foundLeft = false;
            bool foundRight = false;
            // Set the index value of the tangency points to an invalid number (if unchanged then there is an error)
            int upperRightTangency = -1;
            int upperLeftTangency = -1;
            // Until both tangnecy points have been found
            while (!foundTangency)
            {
                // Until a tangency point has been found on the left
                while (!foundLeft)
                {
                    // Grab the next value on the left hull moving counter clockwise
                    nextLeft = left.getNextCounter();
                    // Calculate the new slope of the right anchor point and the new left point
                    double newSlope = getSlope(nextLeft, rightCurrent);
                    // if the new slope is smaller than the old sleep keep rotating
                    if(newSlope < currentSlope)
                    {
                        currentSlope = newSlope;
                        // if we rotate at all we need to check the other side again
                        foundRight = false;
                    }
                    // if the new slope is greater than we have found a tangency point
                    else
                    {
                        // set flag to true
                        foundLeft = true;
                        // rotate the point back clockwise
                        leftCurrent = left.getNextClock();
                        // grab the index of the current point which is the tangency point
                        upperLeftTangency = left.getCurrentIndex();
                        // if both tangency points have been found break from the loops
                        if (foundLeft && foundRight)
                        {
                            foundTangency = true;
                            break;
                        }
                        // reset the slope to make comparisons on the right hull
                        currentSlope = getSlope(leftCurrent, rightCurrent);
                    }
                }
                // follow the same pattern as the left hull but on the right hull this time
                while (!foundRight)
                {
                    // grab the next point on the right hull, clockwise
                    nextRight = right.getNextClock();
                    // calculate the new slope
                    double newSlope = getSlope(leftCurrent, nextRight);
                    // if the new slope is greater than the old one then keep rotating
                    if (newSlope > currentSlope)
                    {
                        currentSlope = newSlope;
                        // if we rotated at all we need to check the left side again
                        foundLeft = false;
                    }
                    else
                    {
                        // set the flags (check the left side again)
                        foundRight = true;
                        foundLeft = false;
                        // get the previous point to be the tangency point
                        rightCurrent = right.getNextCounter();
                        // grab the index
                        upperRightTangency = right.getCurrentIndex();
                        // calculate the new slope
                        currentSlope = getSlope(leftCurrent, rightCurrent);
                    }
                }
            }
            // return the indices of the two tangency points
            int[] tangency = { upperLeftTangency, upperRightTangency };
            return tangency;
        }

        public int[] findLower(Hull left, Hull right)
        {
            /* Find lower tangency points
               Time complexity: O(n + m) n: points in left hull m: points in right hull
               Space complexity: 
            */
            // Use the same approach as the find upper tangency point method but comparisons are flipped and move in oppposite directions
            System.Drawing.PointF[] leftArray = left.getPoints();
            System.Drawing.PointF[] rightArray = right.getPoints();
            int leftIndex = left.getRightMost();
            int rightIndex = right.getLeftMost();
            System.Drawing.PointF leftCurrent = leftArray[leftIndex];
            System.Drawing.PointF rightCurrent = rightArray[rightIndex];
            left.setCurrentIndex(leftIndex);
            right.setCurrentIndex(rightIndex);
            System.Drawing.PointF nextLeft;
            System.Drawing.PointF nextRight;
            double currentSlope = getSlope(leftCurrent, rightCurrent);

            bool foundTangency = false;
            bool foundLeft = false;
            bool foundRight = false;
            int lowerRightTangency = -1;
            int lowerLeftTangency = -1;
            while (!foundTangency)
            {
                // check in the left hull
                while (!foundLeft)
                { 
                    // for the left hull move in the clockwise direction
                    nextLeft = left.getNextClock();
                    double newSlope = getSlope(nextLeft, rightCurrent);
                    // if the slope is greater than the previous one then keep rotating
                    if (newSlope > currentSlope)
                    {
                        currentSlope = newSlope;
                        foundRight = false;
                    }
                    else
                    {
                        foundLeft = true;
                        leftCurrent = left.getNextCounter();
                        lowerLeftTangency = left.getCurrentIndex();
                        // if both tangency points are found and neither have been changed in the last check then break out
                        if (foundLeft && foundRight)
                        {
                            foundTangency = true;
                            break;
                        }
                        currentSlope = getSlope(leftCurrent, rightCurrent);
                    }
                }
                // check in the right hull
                while (!foundRight)
                {
                    // rotate counter clockwise
                    nextRight = right.getNextCounter();
                    double newSlope = getSlope(leftCurrent, nextRight);
                    // if the new slope is less than the previous keep rotating
                    if (newSlope < currentSlope)
                    {
                        currentSlope = newSlope;
                        foundLeft = false;
                    }
                    else
                    {
                        foundRight = true;
                        foundLeft = false;
                        rightCurrent = right.getNextClock();
                        lowerRightTangency = right.getCurrentIndex();
                        currentSlope = getSlope(leftCurrent, rightCurrent);
                    }
                }
            }
            // once both points are found then return their indices
            int[] tangency = { lowerLeftTangency, lowerRightTangency };
            return tangency;
        }

    }

    class Hull
    {
        // Hull class with indices of the left most and right most points
        int leftMost;
        int rightMost;
        // Pointer to the current point
        int currentIndex;
        // Array of all the points in the hull in clockwise order
        System.Drawing.PointF[] points;

        public Hull(System.Drawing.PointF[] pointList)
        {
            /* Hull constructor
               Time complexity: O(n)
               Space complexity: 
            */
            // When creating a new hull copy in the array of points
            points = pointList;
            // Find the leftmost and rightmost points by iterating over all of them
            double left1 = 1000;
            double right1 = 0;
            for (int i =0; i < points.Length; ++i)
            {
                System.Drawing.PointF point = points[i];
                double left2 = point.X;
                // if the new point is smaller (x coordinate) then replace it
                if (left2 < left1)
                {
                    leftMost = i;
                    left1 = left2;
                }
                // if the new point is greater then replace it
                if (left2 > right1)
                {
                    rightMost = i;
                    right1 = left2;
                }
            }
        }
        // return the length of the point array
        public int getCount()
        {
            return points.Length;
        }
        // return the list of points
        public System.Drawing.PointF[] getPoints()
        {
            return points;
        }
        // return the index of the leftmost point
        public int getLeftMost()
        {
            return leftMost;
        }
        // return the index of the rightmost point
        public int getRightMost()
        {
            return rightMost;
        }
        // special mod function to handle negative numbers
        public int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        // move around the hull in the clockwise direction
        public PointF getNextClock()
        {
            currentIndex = mod(currentIndex + 1, points.Length);
            return points[currentIndex];
        }
        // move around the hull in the counterclockwise direction
        public PointF getNextCounter()
        {
            currentIndex = mod(currentIndex - 1, points.Length);
            return points[currentIndex];
        }
        // return the index of the point pointer
        public int getCurrentIndex()
        {
            return currentIndex;
        }
        // change the index of what is being pointed to
        public void setCurrentIndex(int index)
        {
            currentIndex = index;
        }

    }
}
