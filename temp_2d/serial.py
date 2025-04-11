import json
import re

ROWCOLCOUNT = 12
rowCount = 0

def do():
    total = 0
    r = ""
    g = ""
    b = ""
    colorCount = 1
    with open("colors.txt", "r", encoding="utf-8") as file:
        lines = [line.rstrip() for line in file]

    with open("colors.json", "w") as fi:
        fi.write("{\n")
        for line in lines:
            if line == "":
                return
            if total == 0:
                total = int(line)
            else:
                sp = line.split(", ")
                r = sp[0]
                g = sp[1]
                b = sp[2]
                serialize(fi, r, g, b, colorCount, total)
                colorCount += 1
        fi.write("}")

def serialize(fi, r, g, b, colorCount, total):
    global rowCount
    color = {
        "r": r,
        "g": g,
        "b": b
    }

    if colorCount % ROWCOLCOUNT == 1:
        fi.write("\"Row" + str(rowCount) + "\":{\n")

    col = colorCount%12 - 1
    if col < 0:
        col = 11

    fi.write("\"Color" + str(col) + "\":")
    json.dump(color, fi, indent=4)
    if colorCount == total:
        fi.write("\n}\n")
    elif (colorCount % ROWCOLCOUNT == 0):
        fi.write("\n")
    else:
        fi.write(",\n") #trailing commas break the damn thing

    if colorCount % ROWCOLCOUNT == 0 and colorCount != total:
        fi.write("},\n")
        rowCount += 1

    
def roman(num):
    ROMAN = [
        (10, "x"),
        (9, "ix"),
        (5, "v"),
        (4, "iv"),
        (1, "i")
    ] #lazy approach

    result = ""
    for (arabic, roman) in ROMAN:
        (factor, num) = divmod(num, arabic)
        result += roman * factor
    return result

do() #do the thing

# {
#     setup: Pridyr sits imperiously on a barstool and strokes her beard. “Grew this myself- took 200 years to fill out nicely.”
#     options: {
#         a: “That’s sad. That’s not something to be proud of.”
#         points: 3
#         {
#             i: Pridyr huffs and orders some cider and moldy bread.
#             ii: Pridyr splashes her cider onto your poor beard and tears into her moldy bread, tapping her cup on the bar for a refill.
#         }
#     }
#     orientation: in-right, out-right, middle, scroll-left, out-left
# }