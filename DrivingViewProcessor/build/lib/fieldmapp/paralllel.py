

def find_parallel_line(coord_a, coord_b, distance):
    xa, ya = coord_a
    xb, yb = coord_b
    # Calculate slope of the line passing through points a and b
    slope = (yb - ya) / (xb - xa)
    # Calculate y-intercept of the line passing through point a
    # y_intercept = -slope * xa + ya
    b = ya - slope * xa
    # Calculate y-intercept of the parallel line
    parallel_y_intercept = b + distance
    # parallel_y_intercept = y_intercept + distance
    # Return the equation of the parallel line as a tuple (slope, y-intercept)
    return slope, parallel_y_intercept


def find_intersection(m1, b1, m2, b2):
    # Calculate the x-coordinate of the intersection point
    x = (b2 - b1) / (m1 - m2)
    # Calculate the y-coordinate of the intersection point
    y = m1 * x + b1
    # Return the coordinates of the intersection point as a tuple (x, y)
    return x, y


def find_y_intercept(m, point):
    x, y = point
    return y - m * x

