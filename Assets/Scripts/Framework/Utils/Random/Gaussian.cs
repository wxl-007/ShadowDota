using System;

//产生N(a,b)的数：Math.sqrt(b)*random.nextGaussian()+a
public class GaussianRNG {
	private int iset;
	private double gset;
	private Random r1, r2;

	private static GaussianRNG _gaussian;

	private GaussianRNG()
	{
		r1 = new Random(unchecked((int)DateTime.Now.Ticks));
		r2 = new Random(~unchecked((int)DateTime.Now.Ticks));
		iset = 0;
	}

	public static GaussianRNG getInstance() {
		if(_gaussian == null)
			_gaussian = new GaussianRNG();
		return _gaussian;
	}
	
	public double Next()
	{
		double fac, rsq, v1, v2;    
		if (iset == 0) {
			do {
				v1 = 2.0 * r1.NextDouble() - 1.0;
				v2 = 2.0 * r2.NextDouble() - 1.0;
				rsq = v1*v1 + v2*v2;
			} while (rsq >= 1.0 || rsq == 0.0);
			
			fac = Math.Sqrt(-2.0*Math.Log(rsq)/rsq);
			gset = v1*fac;
			iset = 1;
			return v2*fac;
		} else {
			iset = 0;
			return gset;
		}
	}
}

