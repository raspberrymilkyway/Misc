import math

DEFCOLOR = (242, 108, 51)
GENERATE = 144
off = math.floor(256/144)
print(off)
r = DEFCOLOR[0]
g = DEFCOLOR[1]
b = DEFCOLOR[2]

with open("colors.txt", "w") as fi:
    fi.write(str(GENERATE))
    s = str(r) + ", " + str(g) + ", " + str(b)
    fi.write("\n" + s)
    for i in range(GENERATE-1):
        r = (r + off) % 256
        g = (g + off) % 256
        b = (b + off) % 256
        s = str(r) + ", " + str(g) + ", " + str(b)
        fi.write("\n" + s)
    