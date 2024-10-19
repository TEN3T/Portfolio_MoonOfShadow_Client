using System.Collections;
using UnityEngine;

public enum BEAM_TYPE
{
    WIDTH,
    VERTICAL,
    CIRCLE,
    CROSS,
}

public class Destructionstone : FieldStructure
{
    //private const string MAGICPEARL_PATH = "Prefabs/InGame/FieldStructure/FieldStructure_MagicPearl";
    private const string BEAM_PATH = "Prefabs/InGame/FieldStructure/";

    [SerializeField] private BEAM_TYPE type = BEAM_TYPE.WIDTH;

    private SpriteRenderer spriteRenderer;
    private MagicPearl magicPearl;
    private SoundRequesterSFX soundRequester = null;


    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = front.GetComponent<SpriteRenderer>();
        magicPearl = GetComponentInChildren<MagicPearl>();

        if (top.TryGetComponent(out Renderer render))
        {
            render.sortingLayerName = ((LayerConstant)(fieldStructureData.layerOrder)).ToString();
        }
        else if (top.TryGetComponent(out MeshRenderer meshRender))
        {
            meshRender.sortingLayerName = ((LayerConstant)(fieldStructureData.layerOrder)).ToString();
        }
    }

    private void Start()
    {
        soundRequester = GetComponent<SoundRequesterSFX>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!front.enabled)
        {
            return;
        }

        if (collision.transform.parent.TryGetComponent(out Player player))
        {
            StartCoroutine(Activation());
            switch (type)
            {
                case BEAM_TYPE.WIDTH:
                    Beam width = Instantiate(ResourcesManager.Load<Beam>(BEAM_PATH + "BeamWidth"), transform);
                    width.BeamInit(CameraManager.Instance.cam.orthographicSize * CameraManager.Instance.cam.aspect * 2.0f * 2.5f, float.Parse(this.fieldStructureData.gimmickParam[0]), float.Parse(this.fieldStructureData.gimmickParam[1]), float.Parse(this.fieldStructureData.gimmickParam[2]));
                    break;
                case BEAM_TYPE.VERTICAL:
                    Beam vertical = Instantiate(ResourcesManager.Load<Beam>(BEAM_PATH + "BeamVertical"), transform);
                    vertical.BeamInit(float.Parse(this.fieldStructureData.gimmickParam[0]), CameraManager.Instance.cam.orthographicSize * 2.0f * 2.5f, float.Parse(this.fieldStructureData.gimmickParam[1]), float.Parse(this.fieldStructureData.gimmickParam[2]));
                    break;
                case BEAM_TYPE.CIRCLE:
                    Beam circle = Instantiate(ResourcesManager.Load<Beam>(BEAM_PATH + "BeamCircle"), transform);
                    circle.BeamInit(float.Parse(this.fieldStructureData.gimmickParam[0]), float.Parse(this.fieldStructureData.gimmickParam[0]), float.Parse(this.fieldStructureData.gimmickParam[1]), float.Parse(this.fieldStructureData.gimmickParam[2]));
                    break;
                case BEAM_TYPE.CROSS:
                    Beam crossWidth = Instantiate(ResourcesManager.Load<Beam>(BEAM_PATH + "BeamWidth"), transform);
                    Beam crossVertical = Instantiate(ResourcesManager.Load<Beam>(BEAM_PATH + "BeamVertical"), transform);
                    crossWidth.BeamInit(CameraManager.Instance.cam.orthographicSize * CameraManager.Instance.cam.aspect * 2.0f * 2.5f, float.Parse(this.fieldStructureData.gimmickParam[0]), float.Parse(this.fieldStructureData.gimmickParam[1]), float.Parse(this.fieldStructureData.gimmickParam[2]));
                    crossVertical.BeamInit(float.Parse(this.fieldStructureData.gimmickParam[0]), CameraManager.Instance.cam.orthographicSize * 2.0f * 2.5f, float.Parse(this.fieldStructureData.gimmickParam[1]), float.Parse(this.fieldStructureData.gimmickParam[2]));
                    break;
                default:
                    DebugManager.Instance.PrintError("[DestructionStone]: 없는 Type 입니다.");
                    break;
            }
        }
    }

    private IEnumerator Activation()
    {
        front.enabled = false;
        spriteRenderer.enabled = false;
        magicPearl.sprite.enabled = false;
        if (soundRequester != null) {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.ACTIVE);
        }

        yield return new WaitForSeconds(this.fieldStructureData.coolTime);
        front.enabled = true;
        spriteRenderer.enabled = true;
        magicPearl.sprite.enabled = true;
        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.SPAWN);
        }
    }
}
