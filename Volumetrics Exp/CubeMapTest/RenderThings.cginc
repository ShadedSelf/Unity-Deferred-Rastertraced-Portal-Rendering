
float3 HomoToAxis(float3 coords)
{
	float3 normal = abs(coords);
	float great = max(max(normal.x, normal.y), normal.z);
	return ((normal >= (float3)great) ? 1 : 0) * sign(coords);
}

float normalizeFloat(float value, float min, float max)
{
	return (value - min) / (max - min);
}

float3 normalizeFloat3(float3 value, float3 min, float3 max)
{
	return (value - min) / (max - min);
}