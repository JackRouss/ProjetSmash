int Iterations = 128;
float2 Pan = float2(0.5, 0);
float Zoom = 3;
float Aspect = 1;
float2 JuliaSeed = float2(0.39, -0.2);
float3 ColorScale = float3(4, 5, 6);
float4x4 MatrixTransform;
//texture Texture;
//
////-------------------------------- STRUCTURES --------------------------------
//
//// Déclaration de la structure du format de la texture, aucun filtre dans le cas présent
//sampler FormatTexture = sampler_state
//{
//	Texture = (Texture);
//};

void SpriteVertexShader(inout float2 texCoord : TEXCOORD0,
	inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
}

float ComputeValue(float2 v, float2 offset)
{
	float vxsquare = 0;
	float vysquare = 0;

	int iteration = 0;
	int lastIteration = Iterations;

	do
	{
		vxsquare = v.x * v.x;
		vysquare = v.y * v.y;

		v = float2(vxsquare - vysquare, v.x * v.y * 2) + offset;

		iteration++;

		if ((lastIteration == Iterations) && (vxsquare + vysquare) > 4.0)
		{
			lastIteration = iteration + 1;
		}
	} while (iteration < lastIteration);

	return (float(iteration) - (log(log(sqrt(vxsquare + vysquare))) / log(2.0))) / float(Iterations);
}

float4 Mandelbrot_PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
	float2 v = (texCoord - 0.5) * Zoom * float2(1, Aspect) - Pan;

	float val = ComputeValue(v, v);

	return float4(sin(val * ColorScale.x), sin(val * ColorScale.y), sin(val * ColorScale.z), 1);
}

float4 Julia_PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
	float2 v = (texCoord - 0.5) * Zoom * float2(1, Aspect) - Pan;

	float val = ComputeValue(v, JuliaSeed);

	return float4(sin(val * ColorScale.x), sin(val * ColorScale.y), sin(val * ColorScale.z), 1);
}

technique Mandelbrot
{
	pass
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 Mandelbrot_PixelShader();
	}
}

technique Julia
{
	pass
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 Julia_PixelShader();
	}
}