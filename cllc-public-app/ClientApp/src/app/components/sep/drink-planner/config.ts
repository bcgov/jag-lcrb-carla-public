import { faBeer, faGlassMartini, faWineBottle, faWineGlass, IconDefinition } from "@fortawesome/free-solid-svg-icons";

export const HOURS_OF_LIQUOR_SERVICE = 3;
export const SERVINGS_PER_PERSON = 4;

export type DrinkConfig = {
  id: number;
  group: string;
  description: string;
  servingMethod: string;
  storageMethod: string;
  servingSizeMl: number;
  storageSizeMl: number;
  defaultPercentage: number;
  perServingDescription: string;
  imageUrl: string;
  faIcon: IconDefinition;
  servingImageUrl: string;
  storageImageUrl: string;
  storageFaIcon: IconDefinition;
};

const configuration: Array<DrinkConfig> = [
  {
    id: 1,
    group: 'beer',
    description: 'Beer, Ciders & Coolers',
    servingMethod: 'bottles/cans/glasses',
    storageMethod: 'kegs',
    servingSizeMl: 340,
    storageSizeMl: 50000,
    defaultPercentage: 0,
    perServingDescription: '* 12oz / serving',
    imageUrl: 'assets/sep/beer.png',
    faIcon: faBeer,
    storageFaIcon: faBeer,
    servingImageUrl: 'assets/sep/small-beer.png',
    storageImageUrl: 'assets/sep/big-beer.png',
  },
  {
    id: 2,
    group: 'wine',
    description: 'Wine',
    servingMethod: 'glasses',
    storageMethod: 'bottles',
    servingSizeMl: 147,
    storageSizeMl: 750,
    defaultPercentage: 0,
    perServingDescription: '* 5oz / serving',
    imageUrl: 'assets/sep/wine.png',
    faIcon: faWineGlass,
    storageFaIcon: faWineBottle,
    servingImageUrl: 'assets/sep/small-wine.png',
    storageImageUrl: 'assets/sep/big-wine.png',
  },
  {
    id: 3,
    group: 'spirits',
    description: 'Spirits',
    servingMethod: 'shots',
    storageMethod: 'bottles',
    servingSizeMl: 29,
    storageSizeMl: 750,
    defaultPercentage: 0,
    perServingDescription: '* 1oz / serving',
    imageUrl: 'assets/sep/spirits.png',
    faIcon: faGlassMartini,
    storageFaIcon: faWineBottle,
    servingImageUrl: 'assets/sep/small-spirits.png',
    storageImageUrl: 'assets/sep/big-spirits.png',
  }
];

export default configuration;
