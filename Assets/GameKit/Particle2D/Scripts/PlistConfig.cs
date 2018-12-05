using UnityEngine;
using System.Xml;
using UnityEngine.Rendering;

public class PlistConfig : MonoBehaviour {

	public static Particle2DConfig ParsePlistConfig(string plist){
		Particle2DConfig vo = new Particle2DConfig();
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(plist);
		XmlNode root = xmlDoc.SelectSingleNode("plist/dict");
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
		if(config.Name.ToLower().Equals("key")){
			string key = config.InnerText.ToLower();
			config = config.NextSibling as XmlElement;
			string value = config.InnerText;

			if(key.Equals("sourcepositionvariancex")){
				vo.emitterXVariance = float.Parse(value);
			}else if(key.Equals("sourcepositionvariancey")){
				vo.emitterYVariance = float.Parse(value);
			}
			else if(key.Equals("gravityx")){
				vo.gravityX = float.Parse(value);
			}else if(key.Equals("gravityy")){
				vo.gravityY = float.Parse(value);
			}
			else if(key.Equals("emittertype")){
				vo.emitterType = (Particle2D.EmitterType)(float.Parse(value));
			}
			else if(key.Equals("particlelifespan")){
				vo.lifespan = Mathf.Max(0.01f,float.Parse(value));
			}
			else if(key.Equals("particlelifespanvariance")){
				vo.lifespanVariance = float.Parse(value);
			}
			else if(key.Equals("startparticlesize")){
				vo.startSize = float.Parse(value);
			}
			else if(key.Equals("startparticlesizevariance")){
				vo.startSizeVariance = float.Parse(value);
			}
			else if(key.Equals("finishparticlesize")){
				vo.endSize = float.Parse(value);
			}
			else if(key.Equals("finishparticlesizevariance")){
				vo.endSizeVariance = float.Parse(value);
			}
			else if(key.Equals("angle")){
				vo.emitAngle = -Mathf.Deg2Rad*(360f-float.Parse(value));
			}
			else if(key.Equals("anglevariance")){
				vo.emitAngleVariance = -Mathf.Deg2Rad*float.Parse(value);
			}
			else if(key.Equals("rotationstart")){
				vo.startRotation = -Mathf.Deg2Rad*(360f-float.Parse(value));
			}
			else if(key.Equals("rotationstartvariance")){
				vo.startRotationVariance = -Mathf.Deg2Rad*float.Parse(value);
			}
			else if(key.Equals("rotationend")){
				vo.endRotation = -Mathf.Deg2Rad*(360f-float.Parse(value));
			}
			else if(key.Equals("rotationendvariance")){
				vo.endRotationVariance = -Mathf.Deg2Rad*float.Parse(value);
			}
			else if(key.Equals("speed")){
				vo.speed = float.Parse(value);
			}
			else if(key.Equals("speedvariance")){
				vo.speedVariance = float.Parse(value);
			}
			else if(key.Equals("radialacceleration")){
				vo.radialAcceleration = float.Parse(value);
			}
			else if(key.Equals("radialaccelvariance")){
				vo.radialAccelerationVariance = float.Parse(value);
			}
			else if(key.Equals("tangentialacceleration")){
				vo.tangentialAcceleration = float.Parse(value);
			}
			else if(key.Equals("tangentialaccelvariance")){
				vo.tangentialAccelerationVariance = float.Parse(value);
			}
			else if(key.Equals("maxradius")){
				vo.maxRadius = float.Parse(value);
			}
			else if(key.Equals("maxradiusvariance")){
				vo.maxRadiusVariance = float.Parse(value);
			}
			else if(key.Equals("minradius")){
				vo.minRadius = float.Parse(value);
			}
			else if(key.Equals("minradiusvariance")){
				vo.minRadiusVariance = float.Parse(value);
			}
			else if(key.Equals("rotatepersecond")){
				vo.rotatePerSecond = Mathf.Deg2Rad*float.Parse(value);
			}
			else if(key.Equals("rotatepersecondvariance")){
				vo.rotatePerSecondVariance = Mathf.Deg2Rad*float.Parse(value);
			}
			else if(key.Equals("startcolorred")){
				vo.startColor.r = float.Parse(value);
			}else if(key.Equals("startcolorgreen")){
				vo.startColor.g = float.Parse(value);
			}else if(key.Equals("startcolorblue")){
				vo.startColor.b = float.Parse(value);
			}else if(key.Equals("startcoloralpha")){
				vo.startColor.a = float.Parse(value);
			}
			else if(key.Equals("startcolorvariancered")){
				vo.startColorVariance.r = float.Parse(value);
			}else if(key.Equals("startcolorvariancegreen")){
				vo.startColorVariance.g = float.Parse(value);
			}else if(key.Equals("startcolorvarianceblue")){
				vo.startColorVariance.b = float.Parse(value);
			}else if(key.Equals("startcolorvariancealpha")){
				vo.startColorVariance.a = float.Parse(value);
			}
			else if(key.Equals("finishcolorred")){
				vo.endColor.r = float.Parse(value);
			}else if(key.Equals("finishcolorgreen")){
				vo.endColor.g = float.Parse(value);
			}else if(key.Equals("finishcolorblue")){
				vo.endColor.b = float.Parse(value);
			}else if(key.Equals("finishcoloralpha")){
				vo.endColor.a = float.Parse(value);
			}
			else if(key.Equals("finishcolorvariancered")){
				vo.endColorVariance.r = float.Parse(value);
			}else if(key.Equals("finishcolorvariancegreen")){
				vo.endColorVariance.g = float.Parse(value);
			}else if(key.Equals("finishcolorvarianceblue")){
				vo.endColorVariance.b = float.Parse(value);
			}else if(key.Equals("finishcolorvariancealpha")){
				vo.endColorVariance.a = float.Parse(value);
			}
			else if(key.Equals("duration")){
				vo.defaultDuration = float.Parse(value);
				if(vo.defaultDuration<=0) vo.isLooop = true;
			}
			else if(key.Equals("maxparticles")){
				vo.maxParticles = Mathf.FloorToInt(float.Parse(value));
			}
//			else if(key.Equals("blendfuncsource")){
//				vo.srcFactor = GetBlendFunc(int.Parse(value));
//			}
//			else if(key.Equals("blendfuncdestination")){
//				vo.dstFactor = GetBlendFunc(int.Parse(value));
//			}

			// compatibility with future Particle Designer versions
			// (might fix some of the uppercase/lowercase typos)
			if (float.IsNaN(vo.endSizeVariance) && key.Equals("finishparticlesizevariance"))
				vo.endSizeVariance = float.Parse(value);
			if (float.IsNaN(vo.lifespan)&&key.Equals("particlelifespan"))
				vo.lifespan = Mathf.Max(0.01f, float.Parse(value));
			if (float.IsNaN(vo.lifespanVariance) && key.Equals("particlelifespanvariance"))
				vo.lifespanVariance = float.Parse(value);
			if (float.IsNaN(vo.minRadiusVariance))
				vo.minRadiusVariance = 0.0f;
			
		}
	}
}
