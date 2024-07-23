import random
from string import ascii_letters, digits
from datetime import datetime

# Kylee Willis / kwcnr

# basic data generator
# to be used in cloud connection (generate junk data to send)

class DataGenerator:

    def __init__(self):
        self.used_ids = []

    #generate "sensor" data -- temperature, soil readings, precipitation
    # basically returns dictionary/json-format
    def sensor(self,temp=False, soil=False, precipitation=False):
        id = self.__gen_id()
        ret = {"id": id}

        ret["location"] = "("+ str(random.randint(0, 25)) +", " + str(random.randint(0,25)) + ")"
        ret["time"] = datetime.now().strftime("%m/%d/%y %H:%M:%S")

        if temp:
            ret["temp"] = random.randint(-20, 105)
        if soil:
            ret["acidity"] = random.randint(0,14) #should soil acidity be top/bottom of the ph scale? no. but the range technically exists
            ret["nutrients"] = [random.choice(["very low", "low", "average", "high", "very high"]) for i in range(6)] #doesn't really matter but NPKCaMgS
        if precipitation:
            ret["water-level"] = round(random.uniform(0.0, 20.0), 1)

        return ret

    #generate "timer" data -- irrigation timer, pesticide timer
    # i didn't want to set up an actual timer, so they just check random values
    def timer(self, irrigation=False, pesticide=False):
        id = self.__gen_id()
        ret = {"id": id}

        ret["time"] = datetime.now().strftime("%m/%d/%y %H:%M:%S")

        if irrigation:
            ret["watered?"] = random.randint(0,100) == 42
        if pesticide:
            ret["sprayed?"] = random.randint(0,1000) == 444
        
        return ret

    #generate "monitor" data -- bug identifier, wildlife identifier
    # i think these are mostly done with like... cameras and traps? i don't know that it's
    # super viable for a typical farmer, but i thought i'd include it anyway
    def monitor(self, bug=False, wildlife=False):
        id = self.__gen_id()
        ret = {"id": id}

        ret["location"] = "("+ str(random.randint(0, 25)) +", " + str(random.randint(0,25)) + ")"
        ret["time"] = datetime.now().strftime("%m/%d/%y %H:%M:%S")

        if bug:
            ret["type"] = random.choice(["mites", "sawfly", "borer", "weevil", "cutworm"])
        if wildlife:
            ret["species"] = random.choice(["deer", "bear", "moose", "duck", "mouse", "crow"])

        return ret
    
    # generate random id to be used as identifier. used internally.
    #  this is also possibly done at run time when the data is uploaded, but
    #      it can be filtered like this, as needed.
    def __gen_id(self, length=10):
        id = ""
        x = True

        while x:
            for i in range(length):
                # first character should be a letter, not numeric
                if (length == 0):
                    id += random.choice(ascii_letters)
                id += random.choice(ascii_letters + digits)

            if id in self.used_ids:
                id = ""
            else:
                self.used_ids.append(id)
                x = False

        return id


# basically, one dg.(function) would be used per sensor. see below for example usage

# dg = DataGenerator()
# print(dg.sensor(True, True, True))
# print(dg.timer(True, True))
# print(dg.monitor(True, True))