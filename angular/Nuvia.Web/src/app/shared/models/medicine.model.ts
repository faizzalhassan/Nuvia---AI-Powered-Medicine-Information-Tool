export interface RelatedVariant {
  name: string;
  form: string;
  strength: string;
}

export interface MedicineInfo {
  brandName: string;
  genericName: string;
  drugClass: string;
  manufacturer: string;
  strength: string;
  form: string;
  route: string;
  whatIsIt: string;
  usedFor: string[];
  sideEffects: string[];
  warnings: string[];
  directions: string[];
  relatedVariants: RelatedVariant[];
  disclaimer: string;
  found: boolean;
  notFoundMessage: string;
}