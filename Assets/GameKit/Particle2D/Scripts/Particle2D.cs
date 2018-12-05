using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using System.Xml;

[System.Serializable]
public class Particle2DConfig
{
	[Header("Sprite or Texture")]
	public Sprite sprite;
	public Texture texture;
	[Header("Emitter Configuration")]                            // .pex element name
	public Particle2D.EmitterType emitterType = Particle2D.EmitterType.GRAVITY;                       // emitterType
	[Range(0,1000)]
	public int maxParticles=500;						//opacity
	[Range(0f,1000f)]
	public float emitterXVariance = 0;               // sourcePositionVariance x
	[Range(0f,1000f)]
	public float emitterYVariance = 0;               // sourcePositionVariance y
	[Range(0f,60f)]
	public float defaultDuration = 1f;                // duration
	public bool isLooop = true;

	[Header("Particle Configuration")]
	[Range(0f,60f)]
	public float lifespan=2f;                       // particleLifeSpan
	[Range(0f,60f)]
	public float lifespanVariance=1.9f;               // particleLifeSpanVariance
	[Range(0f,600f)]
	public float startSize = 70f;                     // startParticleSize
	[Range(0f,600f)]
	public float startSizeVariance=49.53f;              // startParticleSizeVariance
	[Range(0f,600f)]
	public float endSize =10f;                        // finishParticleSize
	[Range(0f,600f)]
	public float endSizeVariance=5f;                // finishParticleSizeVariance
	[Range(-6.28f,6.28f)]
	public float emitAngle = -4.7123f;                      // angle
	[Range(-6.28f,6.28f)]
	public float emitAngleVariance = -0.0349f;              // angleVariance
	[Range(-6.28f,6.28f)]
	public float startRotation;                  // rotationStart
	[Range(-6.28f,6.28f)]
	public float startRotationVariance;          // rotationStartVariance

	public bool enableEndRotation = true;   	//whether use end rotation
	[Range(-6.28f,6.28f)]
	public float endRotation;                    // rotationEnd
	[Range(-6.28f,6.28f)]
	public float endRotationVariance;            // rotationEndVariance

	[Header("Gravity Configuration")]
	[Range(-1000f,1000f)]
	public float speed = 100f;                          // speed
	[Range(0f,1000f)]
	public float speedVariance=30f;                  // speedVariance
	[Range(-1000f,1000f)]
	public float gravityX;                       // gravity x
	[Range(-1000f,1000f)]
	public float gravityY;                       // gravity y
	[Range(-1000f,1000f)]
	public float radialAcceleration;             // radialAcceleration
	[Range(-1000f,1000f)]
	public float radialAccelerationVariance;     // radialAccelerationVariance
	[Range(-1000f,1000f)]
	public float tangentialAcceleration;         // tangentialAcceleration
	[Range(-1000f,1000f)]
	public float tangentialAccelerationVariance; // tangentialAccelerationVariance

	[Header("Radial Configuration ")]
	[Range(0f,600f)]
	public float maxRadius = 100;                      // maxRadius
	[Range(0f,600f)]
	public float maxRadiusVariance;              // maxRadiusVariance
	[Range(0f,600f)]
	public float minRadius;                      // minRadius
	[Range(0f,600f)]
	public float minRadiusVariance;              // minRadiusVariance
	[Range(0f,720f)]
	public float rotatePerSecond;                // rotatePerSecond
	[Range(0f,720f)]
	public float rotatePerSecondVariance;        // rotatePerSecondVariance

	[Header("Color Configuration")]
	public Color startColor = new Color(1f,0.35935080051422119f,0f,0.61960784313725f);                  // startColor
	public Color startColorVariance= new Color(0,0,0,0);          // startColorVariance
	public Color endColor = new Color(1f,0.35935080051422119f,0f,1f);                    // finishColor
	public Color endColorVariance = new Color(0,0,0,0);            // finishColorVariance
	public bool enableLifeColor = false;
	public Gradient lifeColors;

//	[Header("Blend Function")]
//	public BlendMode srcFactor = BlendMode.SrcAlpha;
//	public BlendMode dstFactor = BlendMode.One;
}

/// <summary>
/// Particle 2d.
/// create by bingheliefeng
/// </summary>
public class Particle2D {

	[System.Serializable]
	public class PDParticle: ParticleVO
	{
		public Color colorArgb;
		public Color colorArgbDelta;
		public float startX, startY;
		public float velocityX, velocityY;
		public float radialAcceleration;
		public float tangentialAcceleration;
		public float emitRadius, emitRadiusDelta;
		public float emitRotation, emitRotationDelta;
		public float rotationDelta;
		public float scaleDelta;
	}
	public enum EmitterType{
		GRAVITY,RADIAL
	}

	[System.Serializable]
	public class ParticleVO
	{
		public float x=0f;
		public float y=0f;
		public float scale=1f;
		public float rotation=0f;
		public Color color = Color.white;
		public float currentTime=0f;
		public float totalTime=1f;
	}
	protected const int MAX_NUM_PARTICLES = 16383;

	protected Mesh m_Mesh;
	protected Vector3[] m_Vertices;
	protected Vector2[] m_Uvs;
	protected Color32[] m_Colors ;
	protected int[] m_Triangles ;
	public Vector3[] vertices{ get { return m_Vertices;} }
	public Vector2[] uvs{ get { return m_Uvs;} }
	public Color32[] colors{ get { return m_Colors;} }
	private Vector2[] m_CellUv = new Vector2[4];

	protected ParticleVO[] m_Particles ;
	protected float m_FrameTime;
	protected int m_NumParticles;

	protected int m_Capacity=100;

	public float emissionRate = 10; //emitted particles per second
	public float emissionTime=0f;
	public float emitterX=0f;
	public float emitterY=0f;
	public Color color=Color.white;

	public Material material;
	public Space simulationSpace = Space.World;

	private Bounds m_bounds;

	public System.Action OnComplete =null;

	private Particle2DConfig m_particleConfigVO;
	public Particle2DConfig config{
		get { return m_particleConfigVO; }
	}

	private Transform m_transform;
	private bool m_isOver = false;
	public bool isOver{
		get { return m_isOver;}
	}

	private bool m_IsPlayed = false;
	public bool isPlaying{
		get { return m_IsPlayed; }
	}

	private float textureWidth{
		get{
			if(m_particleConfigVO.sprite) return m_particleConfigVO.sprite.rect.width;
			else if(m_particleConfigVO.texture) return m_particleConfigVO.texture.width;
			return 2f;
		}
	}
	private float textureHeight{
		get{
			if(m_particleConfigVO.sprite) return m_particleConfigVO.sprite.rect.height;
			else if(m_particleConfigVO.texture) return m_particleConfigVO.texture.height;
			return 2f;
		}
	}

	public void Init(Transform trans,bool isPlayOnAwake=true){
		m_transform = trans;
		m_Mesh = new Mesh();
		m_Mesh.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;	
		m_Mesh.MarkDynamic();
		m_FrameTime = 0.0f;
		emitterX = 0f;
		emitterY = 0f;
		if(isPlayOnAwake){
			emissionTime = float.MaxValue;
			m_IsPlayed = true; 
		}
		emissionRate = 10f;
		m_bounds = new Bounds(m_transform.position,new Vector3(10000f,10000f,0.1f));
	}

	public void SetParticle2DConfigVO(Particle2DConfig config,bool prewarm = false){
		m_particleConfigVO = config;
		if(m_particleConfigVO.isLooop){
			emissionTime = float.MaxValue;
		}else{
			emissionTime = m_particleConfigVO.defaultDuration;
		}
		capacity = m_particleConfigVO.maxParticles;
		UpdateEmissionRate();
		if(Application.isPlaying && prewarm && m_particleConfigVO.isLooop){
			float lifespan = m_particleConfigVO.lifespan + m_particleConfigVO.lifespanVariance * (Random.value * 2.0f - 1.0f);
			if(lifespan<0f) lifespan=0f;
			AdvanceTime(lifespan);
		}
	}
	protected virtual ParticleVO CreateParticle()
	{
		return new PDParticle();
	}

	protected virtual void InitParticle(ParticleVO aParticle)
	{
		PDParticle particle = aParticle as PDParticle; 

		// for performance reasons, the random variances are calculated inline instead
		// of calling a function

		float lifespan = m_particleConfigVO.lifespan + m_particleConfigVO.lifespanVariance * (Random.value * 2.0f - 1.0f);

		particle.currentTime = 0.0f;
		particle.totalTime = lifespan > 0.0f ? lifespan : 0.0f;

		if (lifespan <= 0.0f) return;

		float emitterX = this.emitterX;
		float emitterY = this.emitterY;

		if(m_transform!=null &&  (simulationSpace== Space.World && m_particleConfigVO.emitterType!=EmitterType.RADIAL)){
			Vector2 help = m_transform.InverseTransformPoint(emitterX,emitterY,0);
			emitterX = -help.x;
			emitterY = -help.y;
		}

		particle.x = emitterX + m_particleConfigVO.emitterXVariance * (Random.value * 2.0f - 1.0f);
		particle.y = emitterY + m_particleConfigVO.emitterYVariance * (Random.value * 2.0f - 1.0f);

		particle.startX = emitterX;
		particle.startY = emitterY;

		float angle = m_particleConfigVO.emitAngle + m_particleConfigVO.emitAngleVariance * (Random.value * 2.0f - 1.0f);
		float speed = m_particleConfigVO.speed + m_particleConfigVO.speedVariance * (Random.value * 2.0f - 1.0f);
		particle.velocityX = speed * Mathf.Cos(angle);
		particle.velocityY = speed * Mathf.Sin(angle);

		float startRadius = m_particleConfigVO.maxRadius + m_particleConfigVO.maxRadiusVariance * (Random.value * 2.0f - 1.0f);
		float endRadius   = m_particleConfigVO.minRadius + m_particleConfigVO.minRadiusVariance * (Random.value * 2.0f - 1.0f);
		particle.emitRadius = startRadius;
		particle.emitRadiusDelta = (endRadius - startRadius) / lifespan;
		particle.emitRotation = m_particleConfigVO.emitAngle + m_particleConfigVO.emitAngleVariance * (Random.value * 2.0f - 1.0f);
		particle.emitRotationDelta = m_particleConfigVO.rotatePerSecond + m_particleConfigVO.rotatePerSecondVariance * (Random.value * 2.0f - 1.0f);
		particle.radialAcceleration = m_particleConfigVO.radialAcceleration + m_particleConfigVO.radialAccelerationVariance * (Random.value * 2.0f - 1.0f);
		particle.tangentialAcceleration = m_particleConfigVO.tangentialAcceleration + m_particleConfigVO.tangentialAccelerationVariance * (Random.value * 2.0f - 1.0f);

		float startSize = m_particleConfigVO.startSize + m_particleConfigVO.startSizeVariance * (Random.value * 2.0f - 1.0f);
		float endSize = m_particleConfigVO.endSize + m_particleConfigVO.endSizeVariance * (Random.value * 2.0f - 1.0f);
		if (startSize < 0.1f) startSize = 0.1f;
		if (endSize < 0.1f)   endSize = 0.1f;

		if(m_particleConfigVO!=null && m_particleConfigVO.texture!=null){
			particle.scale = startSize/textureWidth;
			particle.scaleDelta = ((endSize - startSize) / lifespan)/textureWidth;
		}else{
			particle.scale = 1f;
			particle.scaleDelta = 0f;
		}

		// colors
		Color startColor = particle.colorArgb;
		Color colorDelta = particle.colorArgbDelta;

		startColor.r   = m_particleConfigVO.startColor.r;
		startColor.g = m_particleConfigVO.startColor.g;
		startColor.b  = m_particleConfigVO.startColor.b;
		startColor.a = m_particleConfigVO.startColor.a;

		if (m_particleConfigVO.startColorVariance.r != 0)   startColor.r   += m_particleConfigVO.startColorVariance.r * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.startColorVariance.g != 0) startColor.g += m_particleConfigVO.startColorVariance.g * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.startColorVariance.b != 0)  startColor.b  += m_particleConfigVO.startColorVariance.b * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.startColorVariance.a != 0) startColor.a += m_particleConfigVO.startColorVariance.a * (Random.value * 2.0f - 1.0f);
		particle.colorArgb = startColor;

		float endColorRed   = m_particleConfigVO.endColor.r;
		float endColorGreen = m_particleConfigVO.endColor.g;
		float endColorBlue  = m_particleConfigVO.endColor.b;
		float endColorAlpha = m_particleConfigVO.endColor.a;

		if (m_particleConfigVO.endColorVariance.r != 0)   endColorRed   += m_particleConfigVO.endColorVariance.r * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.endColorVariance.g != 0) endColorGreen += m_particleConfigVO.endColorVariance.g * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.endColorVariance.b != 0)  endColorBlue  += m_particleConfigVO.endColorVariance.b * (Random.value * 2.0f - 1.0f);
		if (m_particleConfigVO.endColorVariance.a != 0) endColorAlpha += m_particleConfigVO.endColorVariance.a * (Random.value * 2.0f - 1.0f);

		colorDelta.r   = (endColorRed   - startColor.r)   / lifespan;
		colorDelta.g = (endColorGreen - startColor.g) / lifespan;
		colorDelta.b  = (endColorBlue  - startColor.b)  / lifespan;
		colorDelta.a = (endColorAlpha - startColor.a) / lifespan;
		particle.colorArgbDelta = colorDelta;

		// rotation
		float startRotation = m_particleConfigVO.startRotation +m_particleConfigVO.startRotationVariance * (Random.value * 2.0f - 1.0f);
		float endRotation   = m_particleConfigVO.endRotation   + m_particleConfigVO.endRotationVariance * (Random.value * 2.0f - 1.0f);

		particle.rotation = startRotation;
		if(m_particleConfigVO.enableEndRotation){
			particle.rotationDelta = (endRotation - startRotation) / lifespan;
		}else{
			particle.rotationDelta = 0f;
		}
	}

	protected virtual void AdvanceParticle(ParticleVO aParticle, float passedTime)
	{
		PDParticle particle = aParticle as PDParticle;

		float restTime = particle.totalTime - particle.currentTime;

		passedTime = restTime > passedTime ? passedTime : restTime;
		particle.currentTime += passedTime;

		if (m_particleConfigVO.emitterType== EmitterType.RADIAL)
		{
			particle.emitRotation += particle.emitRotationDelta * passedTime;
			particle.emitRadius   += particle.emitRadiusDelta   * passedTime;
			particle.x = emitterX - Mathf.Cos(particle.emitRotation) * particle.emitRadius;
			particle.y = emitterY - Mathf.Sin(particle.emitRotation) * particle.emitRadius;
		}
		else
		{
			float distanceX = particle.x - particle.startX;
			float distanceY = particle.y - particle.startY;
			float distanceScalar = Mathf.Sqrt(distanceX*distanceX + distanceY*distanceY);
			if (distanceScalar < 0.01f) distanceScalar = 0.01f;

			float radialX = distanceX / distanceScalar;
			float radialY = distanceY / distanceScalar;
			float tangentialX = radialX;
			float tangentialY = radialY;

			radialX *= particle.radialAcceleration;
			radialY *= particle.radialAcceleration;

			float newY = tangentialX;
			tangentialX = -tangentialY * particle.tangentialAcceleration;
			tangentialY = newY * particle.tangentialAcceleration;

			particle.velocityX += passedTime * (m_particleConfigVO.gravityX + radialX + tangentialX);
			particle.velocityY += passedTime * (m_particleConfigVO.gravityY + radialY + tangentialY);
			particle.x += particle.velocityX * passedTime;
			particle.y += particle.velocityY * passedTime;
		}

		particle.scale += particle.scaleDelta * passedTime;
		particle.rotation += particle.rotationDelta * passedTime;

		particle.colorArgb.r += particle.colorArgbDelta.r* passedTime;
		particle.colorArgb.g += particle.colorArgbDelta.g* passedTime;
		particle.colorArgb.b += particle.colorArgbDelta.b* passedTime;
		particle.colorArgb.a += particle.colorArgbDelta.a* passedTime;

		particle.color = particle.colorArgb;
	}

	private void UpdateEmissionRate()
	{
		if(m_particleConfigVO.lifespan>0f){
			emissionRate = capacity / m_particleConfigVO.lifespan;
		}
	}

	/** Starts the emitter for a certain time. @default infinite time */
	public void Play(float duration=float.NaN)
	{
		m_IsPlayed = true;
		if(float.IsNaN(duration)){
			if (emissionRate != 0){
				if(m_particleConfigVO.isLooop)
					emissionTime = float.MaxValue;
				else
					emissionTime = m_particleConfigVO.defaultDuration;
			}
		}
		else
		{
			if (emissionRate != 0){
				emissionTime = duration;
			}
		}
	}

	/** Stops emitting new particles. Depending on 'clearParticles', the existing particles
         *  will either keep animating until they die or will be removed right away. */
	public void Stop(bool clearParticles=false)
	{
		emissionTime = 0.0f;
		if (clearParticles) Clear();
	}

	/** Initialize the <code>ParticleSystem</code> with particles distributed randomly
         *  throughout their lifespans. */
	public void Populate(int count)
	{
		int maxNumParticles = capacity;
		count = Mathf.Min(count, maxNumParticles - m_NumParticles);
		ParticleVO p;
		for (int i=0; i<count; ++i)
		{
			p = m_Particles[m_NumParticles+i];
			InitParticle(p);
			AdvanceParticle(p, Random.value* p.totalTime);
		}
		m_NumParticles += count;
	}

	/** Removes all currently active particles. */
	public void Clear()
	{
		m_NumParticles = 0;
		for(int i=0;i<m_Capacity;++i){
			int vertexID = i * 4;
			SetVertex(vertexID, 0,0);
			SetVertex(vertexID+1,0,0);
			SetVertex(vertexID+2,0,0);
			SetVertex(vertexID+3,0,0);
		}
		m_Mesh.vertices = m_Vertices;
	}

	// properties
	public bool isEmitting { get{ return emissionTime > 0 && emissionRate > 0;} }
	public int numParticles { get {return m_NumParticles;} }
	public int capacity{
		get { return m_Capacity; }
		set {
			m_Capacity = Mathf.Clamp(value,0,MAX_NUM_PARTICLES);
			m_Particles = new PDParticle[m_Capacity];
			for(int i=0;i<m_Capacity;++i){
				m_Particles[i] = new PDParticle();
			}
			UpdateMesh();
			UpdateEmissionRate();
		}
	}
	public Mesh mesh{
		get { return m_Mesh;}
	}

	/// <summary>
	/// Invoke this function if sprite is changed
	/// </summary>
	public void UpdateUV(){
		if(m_Mesh){
			if(m_particleConfigVO.sprite){
				float w = m_particleConfigVO.texture.width;
				float h = m_particleConfigVO.texture.height;
				Rect rect = m_particleConfigVO.sprite.rect;
				m_CellUv[0] = new Vector2((rect.x+rect.width)/w,(rect.y+rect.height)/h);
				m_CellUv[1] = new Vector2((rect.x+rect.width)/w,rect.y/h);
				m_CellUv[2] = new Vector2(rect.x/w,rect.y/h);
				m_CellUv[3] = new Vector2(rect.x/w,(rect.y+rect.height)/h);
			}else{
				m_CellUv[0] = new Vector2(0f,1f);
				m_CellUv[1] = new Vector2(0f,0f);
				m_CellUv[2] = new Vector2(1f,0f);
				m_CellUv[3] = new Vector2(1f,1f);
			}
			for(int i=0;i<m_Capacity*4;i+=4){
				m_Uvs[i] = m_CellUv[0];
				m_Uvs[i+1] = m_CellUv[1];
				m_Uvs[i+2] = m_CellUv[2];
				m_Uvs[i+3] = m_CellUv[3];
			}
			m_Mesh.uv = m_Uvs;
		}
	}

	protected void UpdateMesh(){
		m_Vertices = new Vector3[m_Capacity*4];
		m_Uvs = new Vector2[m_Capacity*4];
		m_Colors = new Color32[m_Capacity*4];
		m_Triangles = new int[m_Capacity*6];

		if(m_particleConfigVO.sprite){
			float w = m_particleConfigVO.texture.width;
			float h = m_particleConfigVO.texture.height;
			Rect rect = m_particleConfigVO.sprite.rect;
			m_CellUv[0] = new Vector2((rect.x+rect.width)/w,(rect.y+rect.height)/h);
			m_CellUv[1] = new Vector2((rect.x+rect.width)/w,rect.y/h);
			m_CellUv[2] = new Vector2(rect.x/w,rect.y/h);
			m_CellUv[3] = new Vector2(rect.x/w,(rect.y+rect.height)/h);
		}else{
			m_CellUv[0] = new Vector2(0f,1f);
			m_CellUv[1] = new Vector2(0f,0f);
			m_CellUv[2] = new Vector2(1f,0f);
			m_CellUv[3] = new Vector2(1f,1f);
		}

		for(int i=0;i<m_Capacity*4;i+=4){
			m_Vertices[i] = new Vector3(0f,0f,-i*0.00001f);
			m_Vertices[i+1] = new Vector3(0f,0f,-i*0.00001f);
			m_Vertices[i+2] = new Vector3(0f,0f,-i*0.00001f);
			m_Vertices[i+3] = new Vector3(0f,0f,-i*0.00001f);

			m_Uvs[i] = m_CellUv[0];
			m_Uvs[i+1] = m_CellUv[1];
			m_Uvs[i+2] = m_CellUv[2];
			m_Uvs[i+3] = m_CellUv[3];

			m_Colors[i] = Color.white;
			m_Colors[i+1] = Color.white;
			m_Colors[i+2] = Color.white;
			m_Colors[i+3] = Color.white;

			int j=i/4*6;
			m_Triangles[j] = i+2;
			m_Triangles[j+1] = i+1;
			m_Triangles[j+2] = i;
			m_Triangles[j+3] = i;
			m_Triangles[j+4] = i+3;
			m_Triangles[j+5] = i+2;
		}
		m_Mesh.vertices = m_Vertices;
		m_Mesh.uv = m_Uvs;
		m_Mesh.triangles = m_Triangles;
		m_Mesh.colors32=m_Colors;
	}

	public void AdvanceTime(float passedTime)
	{
		int particleIndex = 0;
		ParticleVO particle;
		int maxNumParticles = capacity;

		// advance existing particles
		while (particleIndex < m_NumParticles)
		{
			particle = m_Particles[particleIndex];

			if (particle.currentTime < particle.totalTime)
			{
				AdvanceParticle(particle, passedTime);
				++particleIndex;
			}
			else
			{
				if (particleIndex != m_NumParticles - 1)
				{
					ParticleVO nextParticle = m_Particles[m_NumParticles-1];
					m_Particles[m_NumParticles-1] = particle;
					m_Particles[particleIndex] = nextParticle;

					int id = (m_NumParticles-1) * 4;
					SetVertex(id, 0,0);
					SetVertex(id+1,0,0);
					SetVertex(id+2,0,0);
					SetVertex(id+3,0,0);
				}
				--m_NumParticles;
				if (m_NumParticles == 0 && emissionTime == 0 && OnComplete!=null)
					OnComplete();
			}
		}

		// create and advance new particles
		if (emissionTime > 0)
		{
			float timeBetweenParticles = 1.0f / emissionRate;
			m_FrameTime += passedTime;
			while (m_FrameTime > 0)
			{
				if (m_NumParticles < maxNumParticles)
				{
					particle = m_Particles[m_NumParticles];
					InitParticle(particle);
					// particle might be dead at birth
					if (particle.totalTime > 0.0f)
					{
						AdvanceParticle(particle, m_FrameTime);
						++m_NumParticles;
					}
				}
				m_FrameTime -= timeBetweenParticles;
			}
			if (emissionTime != float.MaxValue)
				emissionTime = emissionTime > passedTime ? emissionTime - passedTime : 0.0f;
			if (m_NumParticles == 0 && emissionTime == 0 && OnComplete!=null)
				OnComplete();
		}

		// update vertex data
		int vertexID = 0;
		float rotation;
		float x, y;
		float offsetX, offsetY;
		float pivotX = textureWidth *0.5f ;
		float pivotY = textureHeight*0.5f ;

		Vector3 offset = Vector3.zero;
		if(m_transform!=null && (simulationSpace== Space.World && m_particleConfigVO.emitterType!=EmitterType.RADIAL)) 
			offset = m_transform.InverseTransformPoint(Vector3.zero);

		Color c;
		for (int i=0; i<m_NumParticles; ++i)
		{
			vertexID = i * 4;
			particle = m_Particles[i];
			rotation = particle.rotation;
			offsetX = pivotX*particle.scale;
			offsetY = pivotY*particle.scale;
			x = particle.x+offset.x;
			y = particle.y+offset.y;

			c = particle.color;
			c.r*=color.r;
			c.g*=color.g;
			c.b*=color.b;
			c.a*=color.a;
			if(m_particleConfigVO.enableLifeColor && particle.totalTime>0f){
				float ts = particle.currentTime/particle.totalTime;
				Color lc=m_particleConfigVO.lifeColors.Evaluate(ts);
				c.r*=lc.r;
				c.g*=lc.g;
				c.b*=lc.b;
				c.a*=lc.a;
			}

			m_Colors[vertexID] = c;
			m_Colors[vertexID+1] = c;
			m_Colors[vertexID+2] = c;
			m_Colors[vertexID+3] = c;

			if (rotation!=0f)
			{
				float cos  = Mathf.Cos(rotation);
				float sin  = Mathf.Sin(rotation);
				float cosX = cos * offsetX;
				float cosY = cos * offsetY;
				float sinX = sin * offsetX;
				float sinY = sin * offsetY;

				SetVertex(vertexID,x - cosX - sinY, y - sinX + cosY);
				SetVertex(vertexID+1, x - cosX + sinY, y - sinX - cosY);
				SetVertex(vertexID+2,x + cosX + sinY, y + sinX - cosY);
				SetVertex(vertexID+3,x + cosX - sinY, y + sinX + cosY);
			}
			else 
			{
				// optimization for rotation == 0
				SetVertex(vertexID, x - offsetX, y + offsetY);
				SetVertex(vertexID+1,x - offsetX, y - offsetY);
				SetVertex(vertexID+2, x + offsetX, y - offsetY);
				SetVertex(vertexID+3, x + offsetX, y + offsetY);
			}
		}
		//update dead particle
		if(m_NumParticles>0){
			if(emissionTime==0f){
				for(int i=m_NumParticles;i<m_Capacity;++i){
					vertexID = i * 4;
					SetVertex(vertexID, 0,0);
					SetVertex(vertexID+1,0,0);
					SetVertex(vertexID+2,0,0);
					SetVertex(vertexID+3,0,0);
				}
			}
			m_Mesh.vertices = m_Vertices;
			m_Mesh.colors32 = m_Colors;
			m_bounds.center = m_transform.position;
			m_Mesh.bounds = m_bounds;
			m_isOver = false;
		}else{
			m_isOver = true;
		}
	}

	private void SetVertex(int index,float x, float y){
		Vector3 v= m_Vertices[index];
		v.x = x;
		v.y = y;
		m_Vertices[index] = v;
	}

	public void Destroy(){
		m_Mesh = null;
		m_Uvs = null;
		m_transform =null;
		m_Triangles = null;
		m_Colors = null;
		m_particleConfigVO = null;
		m_Vertices =null;
		m_Particles = null;
	}
}
