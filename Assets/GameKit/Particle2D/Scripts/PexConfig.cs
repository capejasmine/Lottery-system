using UnityEngine;
using System.Xml;
using UnityEngine.Rendering;

public class PexConfig {

	public static Particle2DConfig ParsePexConfig(string pex){
		pex=pex.ToLower();
		Particle2DConfig vo = new Particle2DConfig();
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(pex);
		XmlNode root = xmlDoc.SelectSingleNode("particleemitterconfig");
		XmlNodeList nodeList =root.ChildNodes;
		foreach (XmlNode xn in nodeList)
		{
			XmlElement xe = (XmlElement)xn;
			ParseXMLElement(vo,xe);
		}
		return vo;
	}

	private static void ParseXMLElement(Particle2DConfig vo, XmlElement config)
	{
		if(config.Name.Equals("sourcepositionvariance")){
			vo.emitterXVariance = float.Parse(config.GetAttribute("x"));
			vo.emitterYVariance = float.Parse(config.GetAttribute("y"));
		}
		else if(config.Name.Equals("gravity")){
			vo.gravityX = float.Parse(config.GetAttribute("x"));
			vo.gravityY = -float.Parse(config.GetAttribute("y"));
		}
		else if(config.Name.Equals("emittertype")){
			vo.emitterType = (Particle2D.EmitterType)(int.Parse(config.GetAttribute("value")));
		}
		else if(config.Name.Equals("particlelifespan")){
			vo.lifespan = Mathf.Max(0.01f,float.Parse(config.GetAttribute("value")));
		}
		else if(config.Name.Equals("particlelifespanvariance")){
			vo.lifespanVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("startparticlesize")){
			vo.startSize = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("startparticlesizevariance")){
			vo.startSizeVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("finishparticlesize")){
			vo.endSize = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("finishparticlesizevariance")){
			vo.endSizeVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("angle")){
			vo.emitAngle = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("anglevariance")){
			vo.emitAngleVariance = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotationstart")){
			vo.startRotation = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotationstartvariance")){
			vo.startRotationVariance = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotationend")){
			vo.endRotation = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotationendvariance")){
			vo.endRotationVariance = -Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("speed")){
			vo.speed = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("speedvariance")){
			vo.speedVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("radialacceleration")){
			vo.radialAcceleration = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("radialaccelvariance")){
			vo.radialAccelerationVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("tangentialacceleration")){
			vo.tangentialAcceleration = -float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("tangentialaccelvariance")){
			vo.tangentialAccelerationVariance = -float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("maxradius")){
			vo.maxRadius = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("maxradiusvariance")){
			vo.maxRadiusVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("minradius")){
			vo.minRadius = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("minradiusvariance")){
			vo.minRadiusVariance = float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotatepersecond")){
			vo.rotatePerSecond = Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("rotatepersecondvariance")){
			vo.rotatePerSecondVariance = Mathf.Deg2Rad*float.Parse(config.GetAttribute("value"));
		}
		else if(config.Name.Equals("startcolor")){
			vo.startColor.r = float.Parse(config.GetAttribute("red"));
			vo.startColor.g = float.Parse(config.GetAttribute("green"));
			vo.startColor.b = float.Parse(config.GetAttribute("blue"));
			vo.startColor.a = float.Parse(config.GetAttribute("alpha"));
		}
		else if(config.Name.Equals("startcolorvariance")){
			vo.startColorVariance.r = float.Parse(config.GetAttribute("red"));
			vo.startColorVariance.g = float.Parse(config.GetAttribute("green"));
			vo.startColorVariance.b = float.Parse(config.GetAttribute("blue"));
			vo.startColorVariance.a = float.Parse(config.GetAttribute("alpha"));
		}
		else if(config.Name.Equals("finishcolor")){
			vo.endColor.r = float.Parse(config.GetAttribute("red"));
			vo.endColor.g = float.Parse(config.GetAttribute("green"));
			vo.endColor.b = float.Parse(config.GetAttribute("blue"));
			vo.endColor.a = float.Parse(config.GetAttribute("alpha"));
		}
		else if(config.Name.Equals("finishcolorvariance")){
			vo.endColorVariance.r = float.Parse(config.GetAttribute("red"));
			vo.endColorVariance.g = float.Parse(config.GetAttribute("green"));
			vo.endColorVariance.b = float.Parse(config.GetAttribute("blue"));
			vo.endColorVariance.a = float.Parse(config.GetAttribute("alpha"));
		}
		else if(config.Name.Equals("duration")){
			vo.defaultDuration = float.Parse(config.GetAttribute("value"));
			if(vo.defaultDuration<=0) vo.isLooop = true;
		}
		else if(config.Name.Equals("maxparticles")){
			vo.maxParticles = Mathf.FloorToInt(float.Parse(config.GetAttribute("value")));
		}
//		else if(config.Name.Equals("blendfuncsource")){
//			vo.srcFactor = GetBlendFunc(int.Parse(config.GetAttribute("value")));
//		}
//		else if(config.Name.Equals("blendfuncdestination")){
//			vo.dstFactor = GetBlendFunc(int.Parse(config.GetAttribute("value")));
//		}

		// compatibility with future Particle Designer versions
		// (might fix some of the uppercase/lowercase typos)
		if (float.IsNaN(vo.endSizeVariance) && config.Name.Equals("finishparticlesizevariance"))
			vo.endSizeVariance = float.Parse(config.GetAttribute("value"));
		if (float.IsNaN(vo.lifespan)  && config.Name.Equals("particlelifespan") )
			vo.lifespan = Mathf.Max(0.01f, float.Parse(config.GetAttribute("value")));
		if (float.IsNaN(vo.lifespanVariance) && config.Name.Equals("particlelifespanvariance")  )
			vo.lifespanVariance = float.Parse(config.GetAttribute("value"));
		if (float.IsNaN(vo.minRadiusVariance))
			vo.minRadiusVariance = 0.0f;
	}

	private static BlendMode GetBlendFunc(int value)
	{
		switch (value)
		{
		case 0:     return BlendMode.Zero;
		case 1:     return BlendMode.One;
		case 0x300: return BlendMode.SrcColor;
		case 0x301: return BlendMode.OneMinusSrcAlpha;
		case 0x302: return BlendMode.SrcAlpha;
		case 0x303: return BlendMode.OneMinusSrcAlpha;
		case 0x304: return BlendMode.DstAlpha;
		case 0x305: return BlendMode.OneMinusDstAlpha;
		case 0x306: return BlendMode.DstColor;
		case 0x307: return BlendMode.OneMinusDstColor;
		}
		return BlendMode.SrcAlpha;
	}
}
