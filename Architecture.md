# SCIE - Survivalcraft Industrial Era

整体结构
--------

~~~
SCIE
├─Blocks                       方块物品类
│  ├─4DirectionalMachines      4方向机器
│  ├─6DirectionalMachines      6方向机器
│  ├─BasaltBlock               矿物相关
│  ├─Bullet2Block              步枪子弹
│  ├─BulletBlock               子弹
│  ├─CommonItems               基础物品
│  ├─DrillBlock                挖掘机
│  ├─DrillerElectricElement    挖掘机电路元素
│  ├─ElectricComponents        电子元件
│  ├─ElementBlock              电路元素方块
│  ├─Gunpowder                 火药类
│  ├─ItemBlock                 物品系统
│  ├─Items                     所有物品
│  ├─LiquidPumpElectricElement 液体泵电路元素
│  ├─MachineToolBlock          机床方块
│  ├─Materials                 基本材料
│  ├─MetalBlock                金属块
│  ├─MultiDirectionalBlock     多方向方块
│  ├─Musket2Block              步枪
│  ├─Oil                       石油相关
│  ├─OresAndPowder             矿石和粉末
│  ├─RailBlock                 铁轨方块
│  ├─Scraps                    废料
│  ├─SteelTools                钢工具
│  ├─TankBlock                 油箱、分馏塔、减压器方块
│  └─Vehicles                  交通工具
│
├─Chemistry                化学模块
│  ├─Atom                  原子相关
│  ├─ChemicalBlock         化学相关方块
│  ├─Compound              单质和化合物类
│  ├─DispersionSystem      分散系相关
│  └─Equation              方程式相关
│
├─Common                   公共模块
│  ├─I18N                  国际化相关
│  ├─Utils                 公用函数
│  └─Interfaces            模块目录
│
├─Circuit                  电路相关
│  ├─CircuitDevices        电器
│  ├─CircuitElements       电路元素类
│  ├─Elements              电路元素相关
│  ├─Generator             发电机
│  ├─LockFreeQueue         无锁队列
│  ├─Pipe                  管道相关
│  ├─SubsystemCircuit      电路系统核心
│  └─WireDevice            电线相关
│
├─Components               组件
│  ├─ComponentAirship            飞艇组件
│  ├─ComponentBlastFurnace       高炉组件
│  ├─ComponentBoatI              蒸汽船组件
│  ├─ComponentCastMach           铸造机组件
│  ├─ComponentCoven              焦炉组件
│  ├─ComponentCReactor           化学反应舱组件
│  ├─ComponentDriller            挖掘机组件
│  ├─ComponentElectricFurnace    电阻炉组件
│  ├─ComponentEngine             蒸汽机组件
│  ├─ComponentEngineA            飞艇发动机组件
│  ├─ComponentFireBox            预热炉组件
│  ├─ComponentFurnaceN           铁炉组件
│  ├─ComponentHearthFurnace      平炉组件
│  ├─ComponentPMachs             加工类机器组件
│  ├─ComponentLargeCraftingTable 机床组件
│  ├─ComponentLiquidPump         液体泵组件
│  ├─ComponentMachine            机器基础组件
│  ├─ComponentMagnetizer         磁化机组件
│  ├─ComponentNewChest           冰箱组件
│  ├─ComponentNGui               GUI组件
│  ├─ComponentPlayer             玩家组件
│  ├─ComponentPresser            压板机组件
│  ├─ComponentPresserNN          枪管机组件
│  ├─ComponentSeperator          分离机组件
│  ├─ComponentSqueezer           压榨机组件
│  ├─ComponentTrain              火车及车厢组件
│  ├─ComponentUnloader           放置机组件
│  └─ComponentVariant            生物变异相关组件
│
├─en-us                    英文资源
│
├─IndustrialMod            pak资源
│
├─Subsystems               子系统
│  ├─Subsystem4DirectionalMachinesBehavior 4方向机器方块行为
│  ├─SubsystemBlastBlowerBlockBehavior     鼓风机方块行为
│  ├─SubsystemBlastFurnaceBlockBehavior    高炉焦炉平炉方块行为
│  ├─SubsystemBullet2BlockBehavior         步枪子弹方块行为
│  ├─SubsystemCollapsingWaterBlockBehavior （待测试）
│  ├─SubsystemCrusherBlockBehavior         矿物处理机方块行为
│  ├─SubsystemDiversionBlockBehavior       换向器方块行为
│  ├─SubsystemDrillerBlockBehavior         挖掘机方块行为
│  ├─SubsystemFurnaceNBlockBehavior        炉类方块行为
│  ├─SubsystemInventoryBlockBehavior       容器实体方块行为
│  ├─SubsystemItemBlockBehavior            物品方块行为
│  ├─SubsystemLiquidPumpBlockBehavior      液体泵方块行为
│  ├─SubsystemMachineToolBlockbehavior     机床方块行为
│  ├─SubsystemMineral                      矿物生成
│  ├─SubsystemMusket2BlockBehavior         步枪方块行为
│  ├─SubsystemNRotBlockBehavior            腐烂方块行为
│  ├─SubsystemNSoilBlockBehavior           耕地方块行为
│  ├─SubsystemRailBlockBehavior            铁轨方块行为
│  └─SubsystemSourceOfFireBlockBehavior    火源方块行为
│
├─Widgets               部件
│  ├─BlastFurnaceWidget      高炉界面
│  ├─CastMachWidget          铸造机界面
│  ├─CovenWidget             焦炉界面
│  ├─CReactorWidget          化学反应舱界面
│  ├─DrillerWidget           挖掘机界面
│  ├─ElectricFurnaceWidget   电阻炉界面
│  ├─Engine2Widget           蒸汽船界面
│  ├─EngineAWidget           飞艇界面
│  ├─FireBoxWidget           预热炉界面
│  ├─FurnaceNWidget          铁炉
│  ├─HearthFurnaceWidget     平炉界面
│  ├─LiquidPumpWidget        液体泵界面
│  ├─MachineToolWidget       机床界面
│  ├─Musket2Widget           步枪界面
│  ├─NewChestWidget          冰箱界面
│  ├─NewCraftingRecipeWidget 合成配方界面
│  ├─PresserWidget           压板机界面
│  └─SeperatorWidget         分离机界面
│
├─zh-cn                 中文资源
│
├─Architecture.md       本文件
├─IndustrialMod.png     外置材质
├─README.md             说明
├─SCIE.csproj  C#项目
└─SCIE.sln     解决方案
~~~