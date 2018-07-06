package utils


class DataUtils {
	static String alphabet = (('A'..'N')+('P'..'Z')+('a'..'k')+('m'..'z')+('2'..'9')).join() 
	static randomGenerator = new Random()

	static randomInt(def lowVal, def highVal) {
		def randVal = randomGenerator.nextInt(highVal - lowVal)
		return lowVal + randVal
	}

	static randomString(def n) {
        def key
        for(def i=0; i<50; i++) {
             key = randomGenerator.with {
                (1..n).collect { alphabet[ nextInt( alphabet.length() ) ] }.join()
           }
        }
        return key
    }

    static randomEmail() {
    	def key
    	key = randomString(randomInt(4,8)) + "." + randomString(randomInt(6,12)) + "@" + randomString(randomInt(6,10)) + ".com"
    	return key
    }

    public static void main(String[] args) {
    	System.out.println(DataUtils.randomString(6))
    	System.out.println("" + DataUtils.randomInt(6, 12))
    	System.out.println(DataUtils.randomString(DataUtils.randomInt(6, 12)))
    	System.out.println(DataUtils.randomEmail())
    }
}
